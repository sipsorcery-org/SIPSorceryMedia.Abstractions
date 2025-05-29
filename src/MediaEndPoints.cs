﻿using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SIPSorceryMedia.Abstractions
{
    public delegate void EncodedSampleDelegate(uint durationRtpUnits, byte[] sample);

    public delegate void RawAudioSampleDelegate(AudioSamplingRatesEnum samplingRate, uint durationMilliseconds, short[] sample);

    public delegate void EncodedTextSampleDelegate(byte[] sample);

    public delegate void RawVideoSampleDelegate(uint durationMilliseconds, int width, int height, byte[] sample, VideoPixelFormatsEnum pixelFormat);
    public delegate void VideoSinkSampleDecodedDelegate(byte[] sample, uint width, uint height, int stride, VideoPixelFormatsEnum pixelFormat);

    public delegate void RawVideoSampleFasterDelegate(uint durationMilliseconds, RawImage rawImage); // Avoid to use byte[] to improve performance
    public delegate void VideoSinkSampleDecodedFasterDelegate(RawImage rawImage); // Avoid to use byte[] to improve performance

    public delegate void SourceErrorDelegate(string errorMessage);

    public enum AudioSamplingRatesEnum
    {
        Rate8KHz = 8000,
        Rate16KHz = 16000,
        Rate24kHz = 24000,
        Rate44_1kHz = 44100,
        Rate48kHz = 48000
    }

    public enum VideoPixelFormatsEnum
    {
        Rgb = 0,        // 24 bits per pixel.
        Bgr = 1,        // 24 bits per pixel.
        Bgra = 2,       // 32 bits per pixel.
        I420 = 3,       // 12 bits per pixel.
        NV12 = 4,       // 12 bits per pixel.
        Rgba = 5,       // 32 bits per pixel.
    }

    /// <summary>
    /// A list of standard media formats that can be identified by an ID if
    /// there is no qualifying format attribute provided.
    /// </summary>
    /// <remarks>
    /// For definition of well known types see: https://tools.ietf.org/html/rfc3551#section-6.
    /// </remarks>
    public enum SDPWellKnownMediaFormatsEnum
    {
        PCMU = 0,       // Audio (8000/1).
        GSM = 3,        // Audio (8000/1).
        G723 = 4,       // Audio (8000/1).
        DVI4 = 5,       // Audio (8000/1).
        DVI4_16K = 6,   // Audio (16000/1).
        LPC = 7,        // Audio (8000/1).
        PCMA = 8,       // Audio (8000/1).
        G722 = 9,       // Audio (8000/1).
        L16_2 = 10,     // Audio (44100/2).
        L16 = 11,       // Audio (44100/1).
        QCELP = 12,     // Audio (8000/1).
        CN = 13,        // Audio (8000/1).
        MPA = 14,       // Audio (90000/*).
        G728 = 15,      // Audio (8000/1).
        DVI4_11K = 16,  // Audio (11025/1).
        DVI4_22K = 17,  // Audio (22050/1).
        G729 = 18,      // Audio (8000/1).

        CELB = 24,  // Video (90000).
        JPEG = 26,  // Video (90000).
        NV = 28,    // Video (90000).
        H261 = 31,  // Video (90000).
        MPV = 32,   // Video (90000).
        MP2T = 33,  // Audio/Video (90000).
        H263 = 34,  // Video (90000).
    }

    public static class AudioVideoWellKnown
    {
        public static Dictionary<SDPWellKnownMediaFormatsEnum, AudioFormat> WellKnownAudioFormats =
            new Dictionary<SDPWellKnownMediaFormatsEnum, AudioFormat> {
                { SDPWellKnownMediaFormatsEnum.PCMU,     new AudioFormat(AudioCodecsEnum.PCMU, 0, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.GSM,      new AudioFormat(AudioCodecsEnum.GSM,  3, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.G723,     new AudioFormat(AudioCodecsEnum.G723, 4, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.DVI4,     new AudioFormat(AudioCodecsEnum.DVI4, 5, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.DVI4_16K, new AudioFormat(AudioCodecsEnum.DVI4, 6, 16000, 1)},
                { SDPWellKnownMediaFormatsEnum.LPC,      new AudioFormat(AudioCodecsEnum.LPC,  7, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.PCMA,     new AudioFormat(AudioCodecsEnum.PCMA, 8, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.G722,     new AudioFormat(AudioCodecsEnum.G722, 9, 16000, 8000, 1, null)},
                { SDPWellKnownMediaFormatsEnum.L16_2,    new AudioFormat(AudioCodecsEnum.L16,  10, 44100, 2)},
                { SDPWellKnownMediaFormatsEnum.L16,      new AudioFormat(AudioCodecsEnum.L16,  11, 44100, 1)},
                { SDPWellKnownMediaFormatsEnum.QCELP,    new AudioFormat(AudioCodecsEnum.QCELP,12, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.CN,       new AudioFormat(AudioCodecsEnum.CN,   13, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.MPA,      new AudioFormat(AudioCodecsEnum.MPA,  14, 90000, 1)},
                { SDPWellKnownMediaFormatsEnum.G728,     new AudioFormat(AudioCodecsEnum.G728, 15, 8000, 1)},
                { SDPWellKnownMediaFormatsEnum.DVI4_11K, new AudioFormat(AudioCodecsEnum.DVI4, 16, 11025, 1)},
                { SDPWellKnownMediaFormatsEnum.DVI4_22K, new AudioFormat(AudioCodecsEnum.DVI4, 17, 22050, 1)},
                { SDPWellKnownMediaFormatsEnum.G729,     new AudioFormat(AudioCodecsEnum.G729, 18, 8000, 1)},
            };

        public static Dictionary<SDPWellKnownMediaFormatsEnum, VideoFormat> WellKnownVideoFormats =
           new Dictionary<SDPWellKnownMediaFormatsEnum, VideoFormat> {
                { SDPWellKnownMediaFormatsEnum.CELB,     new VideoFormat(VideoCodecsEnum.CELB, 24, 90000)},
                { SDPWellKnownMediaFormatsEnum.JPEG,     new VideoFormat(VideoCodecsEnum.JPEG, 26, 90000)},
                { SDPWellKnownMediaFormatsEnum.NV,       new VideoFormat(VideoCodecsEnum.NV,   28, 90000)},
                { SDPWellKnownMediaFormatsEnum.H261,     new VideoFormat(VideoCodecsEnum.H261, 31, 90000)},
                { SDPWellKnownMediaFormatsEnum.MPV,      new VideoFormat(VideoCodecsEnum.MPV,  32, 90000)},
                { SDPWellKnownMediaFormatsEnum.MP2T,     new VideoFormat(VideoCodecsEnum.MP2T, 33, 90000)},
                { SDPWellKnownMediaFormatsEnum.H263,     new VideoFormat(VideoCodecsEnum.H263, 34, 90000)}
           };
    }

    public enum AudioCodecsEnum
    {
        PCMU,
        GSM,
        G723,
        DVI4,
        LPC,
        PCMA,
        G722,
        L16,
        QCELP,
        CN,
        MPA,
        G728,
        G729,
        OPUS,

        PCM_S16LE,  // PCM signed 16-bit little-endian (equivalent to FFmpeg s16le). For use with Azure, not likely to be supported in VoIP/WebRTC.

        Unknown
    }

    public enum VideoCodecsEnum
    {
        CELB,
        JPEG,
        NV,
        H261,
        MPV,
        MP2T,
        H263,
        VP8,
        VP9,
        H264,
        H265,

        Unknown
    }

    public enum TextCodecsEnum
    {
        Unknown,
        T140, //T.140 specifies that text and other T.140 elements must be transmitted in ISO 10646-1 code with UTF-8 transformation.
        RED,
    }

    public struct AudioFormat
    {
        public const int DYNAMIC_ID_MIN = 96;
        public const int DYNAMIC_ID_MAX = 127;
        public const int DEFAULT_CLOCK_RATE = 8000;
        public const int DEFAULT_CHANNEL_COUNT = 1;

        public static readonly AudioFormat Empty = new AudioFormat()
        { _isNonEmpty = false, ClockRate = DEFAULT_CLOCK_RATE, ChannelCount = DEFAULT_CHANNEL_COUNT };

        public AudioCodecsEnum Codec { get; set; }

        /// <summary>
        /// The format ID for the codec. If this is a well known codec it should be set to the
        /// value from the codec enum. If the codec is a dynamic it must be set between 96–127
        /// inclusive.
        /// </summary>
        public int FormatID { get; set; }

        /// <summary>
        /// The official name for the codec. This field is critical for dynamic codecs
        /// where it is used to match the codecs in the SDP offer/answer.
        /// </summary>
        public string FormatName { get; set; }

        /// <summary>
        /// The rate used to set RTP timestamps and to be set in the SDP format
        /// attribute for this format. It should almost always be the same as the
        /// <seealso cref="ClockRate"/>. An example of where it's not is G722 which
        /// uses a sample rate of 16KHz but an RTP rate of 8KHz for historical reasons.
        /// </summary>
        /// <example>
        /// In the SDP format attribute below the RTP clock rate is 48000.
        /// a=rtpmap:109 opus/48000/2
        /// </example>
        public int RtpClockRate { get; set; }

        /// <summary>
        /// The rate used by decoded samples for this audio format.
        /// </summary>
        public int ClockRate { get; set; }

        /// <summary>
        /// The number of channels for the audio format.
        /// </summary>
        /// <example>
        /// In the SDP format attribute below the channel count is 2.
        /// Note for single channel codecs the parameter is typically omitted from the
        /// SDP format attribute.
        /// a=rtpmap:109 opus/48000/2
        /// </example>
        public int ChannelCount { get; set; }

        /// <summary>
        /// This is the string that goes in the SDP "a=fmtp" parameter.
        /// This field should be set WITHOUT the "a=fmtp:" prefix.
        /// </summary>
        /// <example>
        /// In the case below this filed should be set as "emphasis=50-15".
        /// a=fmtp:97 emphasis=50-15
        /// </example>
        public string Parameters { get; set; }

        private bool _isNonEmpty;

        /// <summary>
        /// Creates a new audio format based on a well known SDP format.
        /// </summary>
        public AudioFormat(SDPWellKnownMediaFormatsEnum wellKnown) :
            this(AudioVideoWellKnown.WellKnownAudioFormats[wellKnown])
        { }

        /// <summary>
        /// Creates a new audio format based on a well known codec.
        /// </summary>
        public AudioFormat(
            AudioCodecsEnum codec,
            int formatID,
            int clockRate = DEFAULT_CLOCK_RATE,
            int channelCount = DEFAULT_CHANNEL_COUNT,
            string parameters = null) :
            this(codec, formatID, clockRate, clockRate, channelCount, parameters)
        { }

        /// <summary>
        /// Creates a new audio format based on a well known codec.
        /// </summary>
        public AudioFormat(
            AudioCodecsEnum codec,
            int formatID,
            int clockRate,
            int rtpClockRate,
            int channelCount,
            string parameters)
             : this(formatID, codec.ToString(), clockRate, rtpClockRate, channelCount, parameters)
        { }

        /// <summary>
        /// Creates a new audio format based on a dynamic codec (or an unsupported well known codec).
        /// </summary>
        public AudioFormat(
            int formatID,
            string formatName,
            int clockRate = DEFAULT_CLOCK_RATE,
            int channelCount = DEFAULT_CHANNEL_COUNT,
            string parameters = null) :
            this(formatID, formatName, clockRate, clockRate, channelCount, parameters)
        { }

        public AudioFormat(AudioFormat format)
            : this(format.FormatID, format.FormatName, format.ClockRate, format.RtpClockRate, format.ChannelCount, format.Parameters)
        { }

        /// <summary>
        /// Creates a new audio format based on a dynamic codec (or an unsupported well known codec).
        /// </summary>
        public AudioFormat(int formatID, string formatName, int clockRate, int rtpClockRate, int channelCount, string parameters)
        {
            if (formatID < 0)
            {
                // Note format ID's less than the dynamic start range are allowed as the codec list
                // does not currently support all well known codecs.
                throw new ApplicationException("The format ID for an AudioFormat must be greater than 0.");
            }
            else if (formatID > DYNAMIC_ID_MAX)
            {
                throw new ApplicationException($"The format ID for an AudioFormat exceeded the maximum allowed vale of {DYNAMIC_ID_MAX}.");
            }
            else if (string.IsNullOrWhiteSpace(formatName))
            {
                throw new ApplicationException($"The format name must be provided for an AudioFormat.");
            }
            else if (clockRate <= 0)
            {
                throw new ApplicationException($"The clock rate for an AudioFormat must be greater than 0.");
            }
            else if (rtpClockRate <= 0)
            {
                throw new ApplicationException($"The RTP clock rate for an AudioFormat must be greater than 0.");
            }
            else if (channelCount <= 0)
            {
                throw new ApplicationException($"The channel count for an AudioFormat must be greater than 0.");
            }

            FormatID = formatID;
            FormatName = formatName;
            ClockRate = clockRate;
            RtpClockRate = rtpClockRate;
            ChannelCount = channelCount;
            Parameters = parameters;
            _isNonEmpty = true;

            if (Enum.TryParse<AudioCodecsEnum>(FormatName, out var audioCodec))
            {
                Codec = audioCodec;
            }
            else
            {
                Codec = AudioCodecsEnum.Unknown;
            }
        }

        public bool IsEmpty() => !_isNonEmpty;
    }

    public struct VideoFormat
    {
        public const int DYNAMIC_ID_MIN = 96;
        public const int DYNAMIC_ID_MAX = 127;
        public const int DEFAULT_CLOCK_RATE = 90000;

        public static readonly VideoFormat Empty = new VideoFormat()
        { _isNonEmpty = false, ClockRate = DEFAULT_CLOCK_RATE };

        public VideoCodecsEnum Codec { get; set; }

        /// <summary>
        /// The format ID for the codec. If this is a well known codec it should be set to the
        /// value from the codec enum. If the codec is a dynamic it must be set between 96–127
        /// inclusive.
        /// </summary>
        public int FormatID { get; set; }

        /// <summary>
        /// The official name for the codec. This field is critical for dynamic codecs
        /// where it is used to match the codecs in the SDP offer/answer.
        /// </summary>
        public string FormatName { get; set; }

        /// <summary>
        /// The rate used by decoded samples for this video format.
        /// </summary>
        /// <remarks>
        /// Example, 90000 is the clock rate:
        /// a=rtpmap:102 H264/90000
        /// </remarks>
        public int ClockRate { get; set; }

        /// <summary>
        /// This is the "a=fmtp" format parameter that will be set in the SDP offer/answer.
        /// This field should be set WITHOUT the "a=fmtp:0" prefix.
        /// </summary>
        /// <remarks>
        /// Example:
        /// a=fmtp:102 level-asymmetry-allowed=1;packetization-mode=1;profile-level-id=42e01f"
        /// </remarks>
        public string Parameters { get; set; }

        private bool _isNonEmpty;

        /// <summary>
        /// Creates a new video format based on a well known SDP format.
        /// </summary>
        public VideoFormat(SDPWellKnownMediaFormatsEnum wellKnown) :
            this(AudioVideoWellKnown.WellKnownVideoFormats[wellKnown])
        { }

        /// <summary>
        /// Creates a new video format based on a well known codec.
        /// </summary>
        public VideoFormat(VideoCodecsEnum codec, int formatID, int clockRate = DEFAULT_CLOCK_RATE, string parameters = null)
            : this(formatID, codec.ToString(), clockRate, parameters)
        { }

        public VideoFormat(VideoFormat format)
            : this(format.FormatID, format.FormatName, format.ClockRate, format.Parameters)
        { }

        /// <summary>
        /// Creates a new video format based on a dynamic codec (or an unsupported well known codec).
        /// </summary>
        public VideoFormat(int formatID, string formatName, int clockRate = DEFAULT_CLOCK_RATE, string parameters = null)
        {
            if (formatID < 0)
            {
                // Note format ID's less than the dynamic start range are allowed as the codec list
                // does not currently support all well known codecs.
                throw new ApplicationException("The format ID for an VideoFormat must be greater than 0.");
            }
            else if (formatID > DYNAMIC_ID_MAX)
            {
                throw new ApplicationException($"The format ID for an VideoFormat exceeded the maximum allowed vale of {DYNAMIC_ID_MAX}.");
            }
            else if (string.IsNullOrWhiteSpace(formatName))
            {
                throw new ApplicationException($"The format name must be provided for a VideoFormat.");
            }
            else if (clockRate <= 0)
            {
                throw new ApplicationException($"The clock rate for a VideoFormat must be greater than 0.");
            }

            FormatID = formatID;
            FormatName = formatName;
            ClockRate = clockRate;
            Parameters = parameters;
            _isNonEmpty = true;

            if (Enum.TryParse<VideoCodecsEnum>(FormatName, out var videoCodec))
            {
                Codec = videoCodec;
            }
            else
            {
                Codec = VideoCodecsEnum.Unknown;
            }
        }

        public bool IsEmpty() => !_isNonEmpty;
    }

    public struct TextFormat
    {
        public const int DYNAMIC_ID_MIN = 96;
        public const int DYNAMIC_ID_MAX = 127;
        public const int DEFAULT_CLOCK_RATE = 1000;

        public static readonly TextFormat Empty = new TextFormat()
        { ClockRate = DEFAULT_CLOCK_RATE };

        public TextFormat(TextFormat format)
        : this(format.FormatID, format.FormatName, format.ClockRate, format.Parameters)
        { }

        public TextFormat(TextCodecsEnum codec, int formatID, int clockRate = DEFAULT_CLOCK_RATE, string parameters = null)
        : this(formatID, codec.ToString(), clockRate, parameters)
        { }

        /// <summary>
        /// Creates a new text format based on a dynamic codec (or an unsupported well known codec).
        /// </summary>
        public TextFormat(int formatID, string formatName, int clockRate = DEFAULT_CLOCK_RATE, string parameters = null)
        {
            if (formatID < 0)
            {
                // Note format ID's less than the dynamic start range are allowed as the codec list
                // does not currently support all well known codecs.
                throw new ApplicationException("The format ID for an TextFormat must be greater than 0.");
            }
            else if (formatID > DYNAMIC_ID_MAX)
            {
                throw new ApplicationException($"The format ID for an TextFormat exceeded the maximum allowed vale of {DYNAMIC_ID_MAX}.");
            }
            else if (string.IsNullOrWhiteSpace(formatName))
            {
                throw new ApplicationException($"The format name must be provided for a TextFormat.");
            }
            else if (clockRate <= 0)
            {
                throw new ApplicationException($"The clock rate for a TextFormat must be greater than 0.");
            }

            FormatID = formatID;
            FormatName = formatName;
            ClockRate = clockRate;
            Parameters = parameters;

            if (Enum.TryParse<TextCodecsEnum>(FormatName, out var textCodec))
            {
                Codec = textCodec;
            }
            else
            {
                Codec = TextCodecsEnum.Unknown;
            }
        }

        public TextCodecsEnum Codec { get; set; }

        /// <summary>
        /// The format ID for the codec. If this is a well known codec it should be set to the
        /// value from the codec enum. If the codec is a dynamic it must be set between 96–127
        /// inclusive.
        /// </summary>
        public int FormatID { get; set; }

        /// <summary>
        /// The official name for the codec. This field is critical for dynamic codecs
        /// where it is used to match the codecs in the SDP offer/answer.
        /// </summary>
        public string FormatName { get; set; }

        /// <summary>
        /// The rate used by decoded samples for this text format.
        /// </summary>
        /// <remarks>
        /// Example, 1000 is the clock rate:
        /// a=rtpmap:98 t140/1000
        /// </remarks>
        public int ClockRate { get; set; }

        /// <summary>
        /// This is the "a=fmtp" format parameter that will be set in the SDP offer/answer.
        /// This field should be set WITHOUT the "a=fmtp:0" prefix.
        /// </summary>
        /// <remarks>
        /// Example:
        /// a=fmtp:100 98/98/98
        /// </remarks>
        public string Parameters { get; set; }
    }

    public class MediaEndPoints
    {
        public IAudioSource AudioSource { get; set; }
        public IAudioSink AudioSink { get; set; }
        public IVideoSource VideoSource { get; set; }
        public IVideoSink VideoSink { get; set; }
        public ITextSource TextSource { get; set; }
        public ITextSink TextSink { get; set; }
    }

    public interface IAudioEncoder
    {
        /// <summary>
        /// Needs to be set with the list of audio formats that the encoder supports.
        /// </summary>
        List<AudioFormat> SupportedFormats { get; }

        /// <summary>
        /// Encodes 16bit signed PCM samples.
        /// </summary>
        /// <param name="pcm">An array of 16 bit signed audio samples.</param>
        /// <param name="format">The audio format to encode the PCM sample to.</param>
        /// <returns>A byte array containing the encoded sample.</returns>
        byte[] EncodeAudio(short[] pcm, AudioFormat format);

        /// <summary>
        /// Encodes 16bit signed PCM samples.
        /// </summary>
        /// <param name="pcm">An array of 16 bit signed audio samples.</param>
        /// <param name="format">The audio format to encode the PCM sample to.</param>
        /// <param name="destination">A <see cref="IBufferWriter{T}"/> of <see langword="byte"/> to receieve the encoded sample.</param>
        void EncodeAudio(ReadOnlySpan<short> pcm, AudioFormat format, IBufferWriter<byte> destination);

        /// <summary>
        /// Decodes to 16bit signed PCM samples.
        /// </summary>
        /// <param name="encodedSample">The byte array containing the encoded sample.</param>
        /// <param name="format">The audio format of the encoded sample.</param>
        /// <returns>An array containing the 16 bit signed PCM samples.</returns>
        short[] DecodeAudio(byte[] encodedSample, AudioFormat format);
    }

    public interface ITextEncoder
    {
        /// <summary>
        /// Encode a text into a byte array.
        /// </summary>
        /// <param name="text">A symbol or text to be transmitted</param>
        /// <param name="format">The text format of the sample.</param>
        /// <returns>A byte array containing the encoded text sample</returns>
        byte[] EncodeText(char[] text, TextFormat format);

        /// <summary>
        /// Decode a byte array into a string type text.
        /// </summary>
        /// <param name="encodedSample">A symbol or text that was received</param>
        /// <param name="format">The text format of the sample.</param>
        /// <returns></returns>
        char[] DecodeText(byte[] encodedSample, TextFormat format);
    }

    public class RawImage
    {
        /// <summary>
        /// The width, in pixels, of the image
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height, in pixels, of the image
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Integer that specifies the byte offset between the beginning of one scan line and the next.
        /// </summary>
        public int Stride { get; set; }

        /// <summary>
        /// Pointer to an array of bytes that contains the pixel data.
        /// </summary>
        public IntPtr Sample { get; set; }

        /// <summary>
        /// The pixel format of the image
        /// </summary>
        public VideoPixelFormatsEnum PixelFormat { get; set; }

        /// <summary>
        /// Get bytes array of the image.
        /// 
        /// For performance reasons it's better to use directly Sample
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        {
            byte[] result = null;

            if ((Height > 0) && (Stride > 0))
            {
                var bufferSize = Height * Stride;

                result = new byte[bufferSize];
                Marshal.Copy(Sample, result, 0, bufferSize);
            }
            return result;
        }
    }

    public struct VideoSample
    {
        public uint Width;
        public uint Height;
        public byte[] Sample;
    }

    public interface IVideoEncoder : IDisposable
    {
        /// <summary>
        /// Needs to be set with the list of video formats that the encoder supports.
        /// </summary>
        List<VideoFormat> SupportedFormats { get; }

        byte[] EncodeVideo(int width, int height, byte[] sample, VideoPixelFormatsEnum pixelFormat, VideoCodecsEnum codec);

        byte[] EncodeVideoFaster(RawImage rawImage, VideoCodecsEnum codec); // Avoid to use byte[] to improve performance

        void ForceKeyFrame();

        IEnumerable<VideoSample> DecodeVideo(byte[] encodedSample, VideoPixelFormatsEnum pixelFormat, VideoCodecsEnum codec);

        IEnumerable<RawImage> DecodeVideoFaster(byte[] encodedSample, VideoPixelFormatsEnum pixelFormat, VideoCodecsEnum codec); // Avoid to use byte[] to improve performance
    }

    public interface IAudioSource
    {
        event EncodedSampleDelegate OnAudioSourceEncodedSample;

        event Action<uint, ReadOnlyMemory<byte>> OnAudioSourceEncodedSampleEx;

        event RawAudioSampleDelegate OnAudioSourceRawSample;

        event SourceErrorDelegate OnAudioSourceError;

        Task PauseAudio();

        Task ResumeAudio();

        Task StartAudio();

        Task CloseAudio();

        List<AudioFormat> GetAudioSourceFormats();

        void SetAudioSourceFormat(AudioFormat audioFormat);

        void RestrictFormats(Func<AudioFormat, bool> filter);

        void ExternalAudioSourceRawSample(AudioSamplingRatesEnum samplingRate, uint durationMilliseconds, short[] sample);

        bool HasEncodedAudioSubscribers();

        bool IsAudioSourcePaused();
    }

    public interface IAudioSink
    {
        event SourceErrorDelegate OnAudioSinkError;

        List<AudioFormat> GetAudioSinkFormats();

        void SetAudioSinkFormat(AudioFormat audioFormat);

        void GotAudioRtp(IPEndPoint remoteEndPoint, uint ssrc, uint seqnum, uint timestamp, int payloadID, bool marker, byte[] payload);

        void RestrictFormats(Func<AudioFormat, bool> filter);

        Task PauseAudioSink();

        Task ResumeAudioSink();

        Task StartAudioSink();

        Task CloseAudioSink();
    }

    public interface IVideoSource
    {
        event EncodedSampleDelegate OnVideoSourceEncodedSample;

        event RawVideoSampleDelegate OnVideoSourceRawSample;

        event RawVideoSampleFasterDelegate OnVideoSourceRawSampleFaster; // Avoid to use byte[] to improve performance

        event SourceErrorDelegate OnVideoSourceError;

        Task PauseVideo();

        Task ResumeVideo();

        Task StartVideo();

        Task CloseVideo();

        List<VideoFormat> GetVideoSourceFormats();

        void SetVideoSourceFormat(VideoFormat videoFormat);

        void RestrictFormats(Func<VideoFormat, bool> filter);

        void ExternalVideoSourceRawSample(uint durationMilliseconds, int width, int height, byte[] sample, VideoPixelFormatsEnum pixelFormat);

        void ExternalVideoSourceRawSampleFaster(uint durationMilliseconds, RawImage rawImage); // Avoid to use byte[] to improve performance

        void ForceKeyFrame();

        bool HasEncodedVideoSubscribers();

        bool IsVideoSourcePaused();
    }

    public interface IVideoSink
    {
        /// <summary>
        /// This event will be fired by the sink after is decodes a video frame from the RTP stream.
        /// </summary>
        event VideoSinkSampleDecodedDelegate OnVideoSinkDecodedSample;

        event VideoSinkSampleDecodedFasterDelegate OnVideoSinkDecodedSampleFaster; // Avoid to use byte[] to improve performance

        void GotVideoRtp(IPEndPoint remoteEndPoint, uint ssrc, uint seqnum, uint timestamp, int payloadID, bool marker, byte[] payload);

        void GotVideoFrame(IPEndPoint remoteEndPoint, uint timestamp, byte[] payload, VideoFormat format);

        List<VideoFormat> GetVideoSinkFormats();

        void SetVideoSinkFormat(VideoFormat videoFormat);

        void RestrictFormats(Func<VideoFormat, bool> filter);

        Task PauseVideoSink();

        Task ResumeVideoSink();

        Task StartVideoSink();

        Task CloseVideoSink();
    }

    public interface ITextSource
    {
        event EncodedTextSampleDelegate OnTextSourceEncodedSample;
        Task CloseText();
        TextFormat GetTextSourceFormat();
        void SetTextSourceFormat(TextFormat textFormat);
        Task StartText();
        Task PauseText();
        Task ResumeText();
    }

    public interface ITextSink
    {
        Task CloseTextSink();
        void GotTextRtp(IPEndPoint remoteEndPoint, uint ssrc, uint seqnum, uint timestamp, int payloadID, int marker, byte[] payload);
        void SetTextSinkFormat(TextFormat textFormat);
        TextFormat GetTextSinkFormat();
        Task StartTextSink();
        Task PauseTextSink();
        Task ResumeTextSink();
    }
}
