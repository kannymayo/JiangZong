using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pololu.Usc.MaestroEasyExample
{
    class ActionRecorder
    {
        private List<HandFrame> _frames;
        private bool _hasStarted;
        private bool _hasEnded;
        private int  _curFrameIndex;        // for looping through frames during playback

        public ActionRecorder()
        {
            reset();
        }

        public void startRecording()
        {
            reset();
            _hasStarted = true;
        }

        public void endRecording()
        {
            _hasEnded = true;
            calcRelTime();
        }

        public HandFrame getFrame()
        {
            // After the last frame is read, reset iteration index and return null
            if (_curFrameIndex >= _frames.Count)
            {
                _curFrameIndex = 0;
                return null;
            }

            else return _frames[_curFrameIndex++];
        }

        public void record(DateTime time,  Keys action)
        {
            // do nothing if the recorder hasn't started or has ended
            if (!_hasStarted || _hasEnded) return;

            _frames.Add(new HandFrame(time, action));   
        }

        // convert absolute time to relative time for each frame
        private void calcRelTime()
        {
            if (!_hasStarted) return;
            if (_frames.Count < 1) throw new Exception("# of frames smaller than 1");

            // start from the second frame
            for (int i = 0; i < _frames.Count - 1; i++)
            {
                _frames[i].relTime = (int)(_frames[i+1].absTime - _frames[i].absTime).TotalMilliseconds;
            }
        }

        private void reset()
        {
            _hasStarted = false;
            _hasEnded = false;
            _curFrameIndex = 0;
            _frames = new List<HandFrame>();
        }

        
    }

    class HandFrame
    {
        public DateTime absTime;    // absolute time
        public int relTime;         // Milliseconds until next frame
        public Keys action;         // perhaps change to custom ENUM later

        public HandFrame(DateTime absTime, Keys action)
        {
            this.absTime = absTime;
            this.relTime = 0;
            this.action = action;
        }
    }
}
