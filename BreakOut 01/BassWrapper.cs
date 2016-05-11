using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BreakOut_01
{
    public class BassWrapper : IDisposable
    {
        public BassWrapper()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero, Guid.Empty);
        }

        public int Play(string file, bool loop = false)
        {
            int stream = 0;
            Bass.BASS_StreamFree(stream);
            BASSFlag flag = loop ? BASSFlag.BASS_MUSIC_LOOP : BASSFlag.BASS_DEFAULT;
            stream = Bass.BASS_StreamCreateFile(@file, 0L, 0L, flag);
            if (stream != 0)
            {
                Bass.BASS_ChannelPlay(stream, false);
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, 1f);
                return stream;
            }
            return -1;
        }

        public int Play(string file, int Volume, bool loop = false)
        {
            int stream = 0;
            Bass.BASS_StreamFree(stream);
            BASSFlag flag = loop ? BASSFlag.BASS_MUSIC_LOOP : BASSFlag.BASS_DEFAULT;
            stream = Bass.BASS_StreamCreateFile(@file, 0L, 0L, flag);
            if (stream != 0)
            {
                Bass.BASS_ChannelPlay(stream, false);
                Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, (float)Volume / 100f);
                return stream;
            }
            return -1;
        }

        public void Stop(int stream)
        {
            if (IsPlaying(stream))
            {
                Bass.BASS_ChannelStop(stream);
                Bass.BASS_StreamFree(stream);
            }
        }

        public int Level(int stream)
        {
            return Bass.BASS_ChannelGetLevel(stream);
        }

        public void SetVolume(int stream, int Volume)
        {
            double volume = (double)(Volume / 100f);
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, (float)volume);
        }

        public bool IsPlaying(int stream)
        {
            if (Bass.BASS_ChannelIsActive(stream) != BASSActive.BASS_ACTIVE_PLAYING)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Dispose()
        {
            Bass.BASS_Free();
        }
    }
}