using NAudio.Wave;
using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;

namespace ColaLang
{
    class SetSoundVolume : ParserFunction
    {
        public int volume = 100;
        private IWavePlayer waveOut;
        private Mp3FileReader mp3FileReader;
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);
            var vol = Utils.GetSafeInt(args, 0);

            volume = vol;

            return Variable.EmptyInstance;
        }
    }
    class SoundPlayFunction : ParserFunction
    {
        private IWavePlayer waveOut;
        private Mp3FileReader mp3FileReader;
        public override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 1, m_name);
            //Cola.Playsound("path", "type");
            var path = Utils.GetSafeString(args, 0);
            //var type = Utils.GetSafeString(args, 1);

            if (path.EndsWith("wav"))
            {
                SoundPlayer player = new System.Media.SoundPlayer(path);
                player.Play();
            }
            else if (path.EndsWith("mp3"))
            {
                PlayMp3(path);
            }
            else
            {
                Console.WriteLine($"Type {path} not found.");
            }



            return Variable.EmptyInstance;
        }

        private void PlayMp3(string file)
        {
            this.waveOut = new WaveOut();
            this.mp3FileReader = new Mp3FileReader(file);
            this.waveOut.Init(mp3FileReader);
            this.waveOut.Play();
            this.waveOut.PlaybackStopped += WaveOut_PlaybackStopped;
        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            this.waveOut.Dispose();
            this.mp3FileReader.Dispose();
        }
    }
}
