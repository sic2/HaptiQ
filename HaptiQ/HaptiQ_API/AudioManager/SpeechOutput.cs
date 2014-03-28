using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Speech.Synthesis;

namespace HaptiQ_API
{
    public class SpeechOutput
    {
        private const int MAX_SPEED = 5;
        private const int MIN_SPEED = -3;
        private readonly Object _speakLock = new Object();
       
        private SpeechSynthesizer _synth;
        private static SpeechOutput _instance;

        private SpeechOutput()
        {
            _synth = new SpeechSynthesizer();  
            _synth.SelectVoiceByHints(VoiceGender.Female);
            _synth.Rate = 1;
            _synth.Volume = 50; 
        }

        public static SpeechOutput Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SpeechOutput();
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Output sound of given information
        /// </summary>
        /// <param name="information"></param>
        public void speak(String information)
        {
            lock (_speakLock)
            {
                _synth.SpeakAsync(information);
            }
        }

        /// <summary>
        /// Either switch to Male voice or to Female voice.
        /// </summary>
        public void switchVoiceGender()
        {
            lock (_speakLock)
            {
                switch (_synth.Voice.Gender)
                {
                    case VoiceGender.Female:
                        _synth.SelectVoiceByHints(VoiceGender.Male);
                        break;
                    case VoiceGender.Male:
                        _synth.SelectVoiceByHints(VoiceGender.Female);
                        break;
                    default:
                        // Limiting synth voice genders supported for simplicity
                        Helper.Logger("HaptiQ_API.SpeechOutput.switchVoiceGender::Gender not supported");
                        break;
                }
            }
        }

        /// <summary>
        /// Increases the speed of speech up to a certain limit
        /// </summary>
        public void speedUp()
        {
            if (_synth.Rate + 1 <= MAX_SPEED)
            {
                _synth.Rate++;
            }
        }

        /// <summary>
        /// Decreases the speed of speech up to a certain limit
        /// </summary>
        public void slowDown()
        {
            if (_synth.Rate - 1 >= MIN_SPEED)
            {
                _synth.Rate--;
            }
        }

    }
}
