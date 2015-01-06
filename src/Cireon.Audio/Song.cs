﻿using System;

namespace Cireon.Audio
{
    /// <summary>
    /// Wrapper class for background music.
    /// </summary>
    public sealed class Song
    {
        private readonly object streamDisposeMutex = new object();

        private readonly string file;
        private readonly bool prepareBuffer;
        private OggStream stream;

        /// <summary>
        /// True if the stream has been initialised and ready to play.
        /// </summary>
        public bool Prepared
        {
            get { return this.stream.Ready; }
        }

        private bool finished;

        /// <summary>
        /// Whether the song has finished playing.
        /// </summary>
        public bool FinishedPlaying
        {
            get { return this.finished; }
        }

        private float volume;
        /// <summary>
        /// The volume at which the song is played.
        /// </summary>
        public float Volume
        {
            get { return this.volume; }
            set
            {
                this.volume = value;
                if (this.stream != null)
                    this.stream.Volume = this.volume;
            }
        }

        private float pitch;
        /// <summary>
        /// The pitch at which the song is played.
        /// </summary>
        public float Pitch
        {
            get { return this.pitch; }
            set
            {
                this.pitch = value;
                if (this.stream != null)
                    this.stream.Pitch = this.pitch;
            }
        }

        private float lowPassGain;
        /// <summary>
        /// The gain of the low pass filter on this song.
        /// </summary>
        public float LowPassGain
        {
            get { return this.lowPassGain; }
            set
            {
                this.lowPassGain = value;
                if (this.stream != null)
                    this.stream.LowPassGain = this.lowPassGain;
            }
        }

        private bool looping;
        /// <summary>
        /// Whether the song should be looping.
        /// </summary>
        public bool Looping
        {
            get { return this.looping; }
            set
            {
                this.looping = value;
                if (this.stream != null)
                    this.stream.IsLooped = this.looping;
            }
        }

        /// <summary>
        /// Creates a new song from a file.
        /// </summary>
        /// <param name="file">The filename of the ogg-file that contains the music.</param>
        /// <param name="prepareBuffer">Whether the song should prebuffer the first few seconds of music.</param>
        public Song(string file, bool prepareBuffer = false)
        {
            this.file = file;
            this.prepareBuffer = prepareBuffer;

            if (this.prepareBuffer)
                this.initialiseStream();
        }

        private void initialiseStream()
        {
            if (this.stream != null)
                throw new InvalidOperationException("You can not have two streams running at the same time.");

            this.stream = new OggStream(this.file);
            this.stream.Prepare();
            this.stream.Finished += this.onStreamFinish;

            this.stream.Volume = this.volume;
            this.stream.Pitch = this.pitch;
            this.stream.LowPassGain = this.lowPassGain;
            this.stream.IsLooped = this.looping;
        }

        private void onStreamFinish(object sender, EventArgs e)
        {
            if (this.looping)
                return;

            this.finished = true;
            lock (this.streamDisposeMutex)
            {
                this.stream.Dispose();
                this.stream = null;
            }

            if (this.prepareBuffer)
                this.initialiseStream();
        }

        /// <summary>
        /// Starts playing the song.
        /// </summary>
        public void Play()
        {
            this.finished = false;

            lock (this.streamDisposeMutex)
            {
                if (this.stream == null)
                    this.initialiseStream();
                this.stream.Play();
            }
        }

        /// <summary>
        /// Pauses playing the song.
        /// </summary>
        public void Pause()
        {
            lock (this.streamDisposeMutex)
                if (this.stream != null)
                    this.stream.Pause();
        }

        /// <summary>
        /// Stops playing the song.
        /// </summary>
        public void Stop()
        {
            lock (this.streamDisposeMutex)
                if (this.stream != null)
                    this.stream.Stop();
        }

        /// <summary>
        /// Prepares the buffer for playing (if it isn't already).
        /// </summary>
        public void Prepare()
        {
            lock (this.streamDisposeMutex)
                if (this.stream != null)
                    this.initialiseStream();
        }

        /// <summary>
        /// Stops the song and frees up the allocated resources.
        /// </summary>
        public void Dispose()
        {
            this.Stop();

            lock (this.streamDisposeMutex)
                if (this.stream != null)
                    this.stream.Dispose();
        }
    }
}
