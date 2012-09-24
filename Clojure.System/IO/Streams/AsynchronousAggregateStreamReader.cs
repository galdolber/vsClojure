using System;
using System.Threading;

namespace Clojure.System.IO.Streams
{
	public class AsynchronousAggregateStreamReader
	{
		private readonly StreamBuffer _outputStreamBuffer;
		private readonly StreamBuffer _errorStreamBuffer;
		public event Action<string> DataReceived;

		public AsynchronousAggregateStreamReader(StreamBuffer outputStreamBuffer, StreamBuffer errorStreamBuffer)
		{
			_outputStreamBuffer = outputStreamBuffer;
			_errorStreamBuffer = errorStreamBuffer;
		}

		public void StartReading()
		{
			while (true)
			{
				Thread.Sleep(2);

				if (_outputStreamBuffer.HasData && _errorStreamBuffer.HasData) DataReceived(_errorStreamBuffer.GetData());
				if (_outputStreamBuffer.HasData) DataReceived(_outputStreamBuffer.GetData());
				if (_errorStreamBuffer.HasData) DataReceived(_errorStreamBuffer.GetData());
			}
		}
	}
}