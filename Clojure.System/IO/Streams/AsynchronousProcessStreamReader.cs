using System;
using System.Diagnostics;
using System.Threading;

namespace Clojure.System.IO.Streams
{
	public class AsynchronousProcessStreamReader
	{
		private readonly StreamBuffer _outputStreamBuffer;
		private readonly StreamBuffer _errorStreamBuffer;
		private readonly Thread _processOutputThread;
		private readonly Thread _processErrorThread;
		private readonly Thread _aggregateReaderThread;
		public event Action<string> DataReceived;

		public AsynchronousProcessStreamReader(Process process)
		{
			_outputStreamBuffer = new StreamBuffer();
			_errorStreamBuffer = new StreamBuffer();

			_aggregateReaderThread = new Thread(ReadFromStreams);
			_processOutputThread = new Thread(() => _outputStreamBuffer.ReadStream(process.StandardOutput.BaseStream));
			_processErrorThread = new Thread(() => _errorStreamBuffer.ReadStream(process.StandardError.BaseStream));

			process.Exited += (o, e) => StopReading();
		}

		public void StartReading()
		{
			_processOutputThread.Start();
			_processErrorThread.Start();
			_aggregateReaderThread.Start();
		}

		public void StopReading()
		{
			_processOutputThread.Abort();
			_processErrorThread.Abort();
			_aggregateReaderThread.Abort();
		}

		private void ReadFromStreams()
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