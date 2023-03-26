using System;
using System.IO;

namespace Test.wikipedia
{
    public class LimitedInputStream : Stream
    {
        private readonly Stream _stream;
        private readonly long _limit;
        private long _position;

        public LimitedInputStream(Stream stream, long limit)
        {
            _stream = stream;
            _limit = limit;
            _position = 0;
        }

        public override bool CanRead => _position < _limit && _stream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _limit;

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            long remaining = _limit - _position;
            if (remaining <= 0)
            {
                return 0;
            }

            int bytesRead = _stream.Read(buffer, offset, (int)Math.Min(count, remaining));
            _position += bytesRead;
            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}