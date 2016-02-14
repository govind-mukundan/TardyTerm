using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TardyTerm
{
    class HRAnalyzer
    {
        public int C_LSB_MAX = 0xFFFF;
        public int HR;

        static public int SAMPLE_RATE_HZ = 100;
        static public int C_MV_WINDOW_WIDTH_MS = 100;
        static public int C_MV_WINDOW_WIDTH_N = C_MV_WINDOW_WIDTH_MS * SAMPLE_RATE_HZ / 1000;
        static int C_MV_FILT_DELAY = (C_MV_WINDOW_WIDTH_N - 1) / 2;

        //private float[] HPFilterB = new float[] { 0.9890F ,  -1.9779F,    0.9890F };
        //private float[] HPFilterA = new float[] { 1.0000F,   -1.9778F,    0.9780F };
        // SR = 400sps
        //double[] HPFilterB = new double[] { 0.988954248067140F, -1.977908496134280F, 0.988954248067140F };
        //double[] HPFilterA = new double[] { 1.0000F, -1.977786483776764F, 0.978030508491796F };
        // SR = 100sps
        float[] HPFilterB = new float[] { 0.956543225556877F, -1.913086451113754F, 0.956543225556877F };
        float[] HPFilterA = new float[] { 1.0000F, -1.911197067426073F, 0.914975834801434F };

        //float[] HPFilterB = new float[] { 0.9565F, -1.9131F, 0.9565F };
        //float[] HPFilterA = new float[] { 1.0000F, -1.9112F, 0.9150F };

        //float[] HPFilterB = new float[] { 0.9565F, -1.9131F, 0.9565F };
        //float[] HPFilterA = new float[] { 1.0000F, -1.9112F, 0.9150F };

        private float[] HPBuffer = new float[3];
        //private double[] HPBuffer = new double[3];

        // Moving Window
        private int[] dataWW; // Moving Window buffer
        private int ptr_mw;
        int _mwSum = 0;

        // Peak detector
        int _nSinceLastPk;
        int _max;
        int _lastDatum;
        int C_PK_TIMEOUT = SAMPLE_RATE_HZ * 2; // ~ 2S without a peak

        // Decision Rules
        static int C_HPF_BUFFER_SIZE = 1024;
        int[] HPFOpBuffer = new int[C_HPF_BUFFER_SIZE];
        int _HPFOpIdx;
        int C_PK_MIN_AMP = 50;
        int _sigThresh, _sigSum;
        int[] _sigBuff = new int[5];
        int _sigIdx;
        int _nSinceLastPass;
        int C_SIG_THRESH_RESET_TIMEOUT_N = 4; // 4 peaks that don't make it past the threshold

        // Analyze
        UInt32 _sampleCnt;
        UInt32 _lastSampleCnt;

       public HRAnalyzer()
        {
            dataWW = new int[C_MV_WINDOW_WIDTH_N];
            Decide(0, 0, true);
            int delay;
            Peak(0, out delay, true);
        }

        public void Analyze(int datum)
        {
            int delay, pk;
            
            datum = HPF(datum);
            // Store in buffer for later analysis
            HPFOpBuffer[_HPFOpIdx++] = datum;
            if (_HPFOpIdx == HPFOpBuffer.Length) _HPFOpIdx = 0;

            datum = MVInt(Math.Abs(datum));
            pk = Peak(datum, out delay, false);
            if ((pk != 0) && (Decide(pk, _sampleCnt, false) == true))
            {
                int hr = (int)((float)SAMPLE_RATE_HZ * 60.0 / (_sampleCnt - _lastSampleCnt));
                Debug.WriteLine("HR:{0}", hr);
                _lastSampleCnt = _sampleCnt;
                HR = hr; 
            }

            _sampleCnt++;
        }

        public int HPF(int datum)
        {
            int k;
            
            for (k = 0; k < HPFilterB.Length - 1; k++)
            {
                HPBuffer[k] = HPBuffer[k + 1];
            }

            HPBuffer[HPFilterB.Length - 1] = 0.0F;
            for (k = 0; k < HPFilterB.Length; k++)
            {
                HPBuffer[k] += datum * HPFilterB[k];
            }

            for (k = 0; k < HPFilterA.Length - 1; k++)
            {
                HPBuffer[k + 1] -= HPBuffer[0] * HPFilterA[k + 1];
            }

            return ((int)HPBuffer[0]);
        }

        // input should be a +ve value (use abs())
        public int MVInt(int datum)
        {

            int output;

            _mwSum += datum;
            _mwSum -= dataWW[ptr_mw];
            dataWW[ptr_mw] = datum;
            if (++ptr_mw == C_MV_WINDOW_WIDTH_N)
                ptr_mw = 0;
            if ((_mwSum / C_MV_WINDOW_WIDTH_N) > C_LSB_MAX)
                output = C_LSB_MAX;
            else
                output = _mwSum / C_MV_WINDOW_WIDTH_N;
            return (output);
        }

        // Returns non-zero if a peak was detected, along with the delay from true maximum sample
        public int Peak(int datum, out int delay, bool init)
        {
            int pk = 0;
            delay = 0;

            if (init)
            {
                _max = 0;
                _nSinceLastPk = 0;
                _lastDatum = 0;
                return pk;
            }

            _nSinceLastPk++;

            if ((datum > _max) && (datum > _lastDatum))
            {
                _max = datum;
                _nSinceLastPk = 0;
            }
            else if (datum < _max / 2) // falling edge
            {
                pk = _max;
                delay = _nSinceLastPk;
                _nSinceLastPk = 0;
                _max = 0;
                Debug.WriteLine("PK:{0}", pk);
            }
            else if (_nSinceLastPk >= C_PK_TIMEOUT)
            {
                Debug.WriteLine("PK Timeout:{0}", _nSinceLastPk);
                _nSinceLastPk = 0;
                _max = 0;
                
            }

            _lastDatum = datum;

            return pk;
        }

        // Input is the peak height and current sample number
        bool Decide(int pk, UInt32 sampleIdx, bool init)
        {
            if (init)
            {
                _sigThresh = 0;
                _sigIdx = 0;
                _sigSum = 0;
                for (int i = 0; i < _sigBuff.Length; i++)
                {
                    _sigBuff[i] = 0;
                }
                return false;
            }

            // 1. The peak sample index should correspond to a -ve sample at the HPF output
            // 2. Peak height should be greater than detection threshold
            // TODO: more conditions like reject/reset on too large values of peak height
            Debug.WriteLine("Th:{0}", _sigThresh / 2 );
            if (((sampleIdx - C_MV_FILT_DELAY) > 0) && (HPFOpBuffer[(sampleIdx - C_MV_FILT_DELAY) % C_HPF_BUFFER_SIZE] < 0))
            {
                if ((pk > C_PK_MIN_AMP) && (pk > _sigThresh / 2))
                {
                    // Update signal threshold now that we have a valid peak
                    _sigSum += pk;
                    _sigSum -= _sigBuff[_sigIdx];
                    _sigBuff[_sigIdx++] = pk;
                    if (_sigIdx >= _sigBuff.Length) _sigIdx = 0;

                    _sigThresh = _sigSum / _sigBuff.Length; // todo: use MEDIAN
                    _nSinceLastPass = 0;
                    return true;
                }
                else
                {
                    _nSinceLastPass++;
                    if (_nSinceLastPass >= C_SIG_THRESH_RESET_TIMEOUT_N)
                    {
                        _sigThresh = 0;
                        _sigIdx = 0;
                        _sigSum = 0;
                        for (int i = 0; i < _sigBuff.Length; i++)
                        {
                            _sigBuff[i] = 0;
                        }
                    }
                }
            }
            return false;
        }

        /*
        private double NormalizeHPF(double ip)
        {
            double op = 0;
            if (ip > MinValue && ip < MaxValue)
                op = ip;
            else if (ip < 0)
                op = MinValue;
            else
                op = MaxValue;

            return (op);
        }*/

    }
}
