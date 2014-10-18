﻿using System;
using System.IO;

namespace Cireon.Audio
{
    /// <summary>
    /// Wrapper class for sound effects.
    /// </summary>
    public sealed class SoundFile
    {
        private readonly SoundBuffer buffer;

        private SoundFile(SoundBufferData bufferData)
        {
            this.buffer = SoundBuffer.FromData(bufferData);
        }

        /// <summary>
        /// Plays a single loop of the sound effect.
        /// </summary>
        public void Play()
        {
            this.GenerateSource().Play();
        }

        /// <summary>
        /// Generates an audio source, fills its buffers and sets its properties to the default values.
        /// </summary>
        /// <returns>The source filled with the right buffers with the default properties set.</returns>
        public Source GenerateSource()
        {
            var source = AudioManager.Instance.SourceManager.RequestSource();
            source.QueueBuffer(this.buffer);
            source.Volume = AudioManager.Instance.MasterVolume * AudioManager.Instance.EffectsVolume;
            source.Pitch = AudioManager.Instance.Pitch;
            return source;
        }

        #region Static methods
        /// <summary>
        /// Loads a new soundfile from an ogg-file.
        /// </summary>
        /// <param name="file">The filename of the ogg-file that contains the sound effect.</param>
        public static SoundFile FromOgg(string file)
        {
            return new SoundFile(SoundBufferData.FromOgg(file));
        }

        /// <summary>
        /// Loads a new soundfile from an ogg-file.
        /// </summary>
        /// <param name="stream">The filestream containing the sound effect in ogg-format.</param>
        public static SoundFile FromOgg(Stream stream)
        {
            return new SoundFile(SoundBufferData.FromOgg(stream));
        }

        /// <summary>
        /// Loads a new soundfile from an uncompressed wave-file.
        /// </summary>
        /// <param name="file">The filename of the uncompressed wave-file that contains the sound effect.</param>
        public static SoundFile FromWav(string file)
        {
            return new SoundFile(SoundBufferData.FromWav(file));
        }

        /// <summary>
        /// Loads a new soundfile from an uncompressed wave-file.
        /// </summary>
        /// <param name="stream">The filestream containing the sound effect in wave-format.</param>
        public static SoundFile FromWav(Stream stream)
        {
            return new SoundFile(SoundBufferData.FromWav(stream));
        }
        #endregion
    }
}
