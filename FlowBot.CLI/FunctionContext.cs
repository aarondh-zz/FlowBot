using FlowBot.CLI.Interfaces;
using FlowBot.CLI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBot.CLI
{
    public class FunctionContext : IFunctionContext, IDisposable
    {
        private Func<TextReader> _inputProvider;
        private Func<TextWriter> _outputProvider;
        private TextReader _input;
        private TextWriter _output;
        public RestClient Client { get; private set; }
        public ILog Log { get; private set; }
        public Options Options { get; private set; }
        public TextWriter Output
        {
            get
            {
                if (_output == null)
                {
                    _output = _outputProvider();
                }
                return _output;
            }
        }
        public TextReader Input
        {
            get
            {
                if (_input == null)
                {
                    _input = _inputProvider();
                }
                return _input;
            }
        }
        public FunctionContext(Options options, RestClient client, ILog log, Func<TextReader> inputProvider, Func<TextWriter> outputProvider)
        {
            this.Options = options;
            this.Client = client;
            this.Log = log;
            _inputProvider = inputProvider;
            _outputProvider = outputProvider;
        }
        protected void Dispose(bool disposing)
        {
            if (_input != null && _input != Console.In)
            {
                _input.Close();
            }
            if (_output != null && _output != Console.Out)
            {
                _output.Flush();
                _output.Close();
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
        ~FunctionContext()
        {
            Dispose(false);
        }
    }
}
