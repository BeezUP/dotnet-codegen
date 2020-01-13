using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;

namespace DocumentRefLoader.Yaml
{
    public class ForceQuotedStringValuesEventEmitter : ChainedEventEmitter
    {
        private readonly Stack<EmitterState> _state = new Stack<EmitterState>();

        private class EmitterState
        {
            private readonly int _valuePeriod;
            private int _currentIndex;
            public EmitterState(int valuePeriod) { _valuePeriod = valuePeriod; }
            public bool VisitNext() => ((++_currentIndex) % _valuePeriod) == 0;
        }

        public ForceQuotedStringValuesEventEmitter(IEventEmitter nextEmitter) : base(nextEmitter)
        {
            _state.Push(new EmitterState(1));
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (_state.Peek().VisitNext() && eventInfo.Source.Type == typeof(string))
            {
                eventInfo.Style = ScalarStyle.DoubleQuoted;
            }
            base.Emit(eventInfo, emitter);
        }

        public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter)
        {
            _state.Peek().VisitNext();
            _state.Push(new EmitterState(2));
            base.Emit(eventInfo, emitter);
        }
        public override void Emit(MappingEndEventInfo eventInfo, IEmitter emitter)
        {
            _state.Pop();
            base.Emit(eventInfo, emitter);
        }

        public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
        {
            _state.Peek().VisitNext();
            _state.Push(new EmitterState(1));
            base.Emit(eventInfo, emitter);
        }
        public override void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter)
        {
            _state.Pop();
            base.Emit(eventInfo, emitter);
        }
    }
}
