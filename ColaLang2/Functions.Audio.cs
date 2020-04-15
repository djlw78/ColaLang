using NAudio.Wave;
using NAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitAndMerge
{
    class SoundPlayFunction : ParserFunction
    {
        private IWavePlayer waveOut;
        private Mp3FileReader mp3FileReader;
        protected override Variable Evaluate(ParsingScript script)
        {
            List<Variable> args = script.GetFunctionArgs();
            Utils.CheckArgs(args.Count, 2, m_name);

            var path = Utils.GetSafeString(args, 0);
            var type = Utils.GetSafeString(args, 1);

            if (type == "wav")
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(path);
                player.Play();
            }
            else if (type == "mp3")
            {
                PlayMp3(path);
            }
            else
            {
                Console.WriteLine($"Type {type} not found.");
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
