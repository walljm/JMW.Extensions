using System.IO;
using JMW.Parsing.Expressions;
using JMW.Parsing.Handlers;

namespace JMW.Parsing.IO
{
    public class SectionReader
    {
        private StreamReader _rdr;

        private IExpression _start;
        private IExpression _stop;

        private bool _started = false;
        private bool _stopped = false;

        private bool _includeStartLine = false;
        private bool _includeStopLine = false;

        #region Constructors

        public SectionReader(StreamReader stream, IExpression start, IExpression stop) : this(stream, start, stop, false, false)
        {
        }

        public SectionReader(StreamReader stream, Include include)
        {
            _rdr = stream;
            _start = include.Start;
            _stop = include.Stop;
            _includeStartLine = include.IncludeStart;
            _includeStopLine = include.IncludeStop;
        }

        public SectionReader(StreamReader stream, IExpression start, IExpression stop, bool include_start_line, bool include_stop_line)
        {
            _rdr = stream;
            _start = start;
            _stop = stop;
            _includeStartLine = include_start_line;
            _includeStopLine = include_stop_line;
        }

        #endregion Constructors

        #region Overrides

        /// <summary>Reads a line of characters from the current stream and returns the data as a string.</summary>
        /// <returns>The next line from the input stream, or null if the end of the input stream is reached.</returns>
        /// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string. </exception>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <filterpriority>1</filterpriority>
        public string ReadLine()
        {
            if (_start == null && _stop == null)
                return _rdr.ReadLine();

            if (_stopped)
                return null;

            string line;
            while ((line = _rdr.ReadLine()) != null)
            {
                if (!_started && _start != null && !_start.Test(line))
                    continue;

                if (!_stopped && _stop != null && _stop.Test(line))
                {
                    _stopped = true;
                    if (!_includeStopLine)
                        break;
                }

                if (!_started)
                {
                    _started = true;
                    if (!_includeStartLine)
                        continue; // don't include the line that starts the section.
                }

                return line;
            }

            return null;
        }

        #endregion Overrides
    }
}