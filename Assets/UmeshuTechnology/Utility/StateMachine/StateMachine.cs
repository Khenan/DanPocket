//using System;
//using System.Collections;
//using System.Collections.Generic;

//[Serializable]
//public abstract class StateMachine<K, S> : IReadOnlyDictionary<K, S> where K : Enum where S : State<K>
//{
//    public delegate void StateMachineAction(StateMachine<K, S> _stateMachine);
//    public StateMachine(IDictionary<K, S> _statesDictionary, K _state)
//    {
//        states = new(_statesDictionary);
//        currentKey = _state;
//    }
//    ~StateMachine() => onDestroy?.Invoke(this);


//    #region StateMachine_Properties
//    private readonly Dictionary<K, S> states = new();
//    private K currentKey;
//    public S CurrentState => ContainsKey(CurrentKey) ? this[CurrentKey] : null;
//    public K CurrentKey => currentKey;
//    #endregion

//    #region events

//    public event StateMachineAction onDestroy;

//    #endregion

//    #region ReadonlyDictionary_Properties
//    public S this[K _key] => states[_key];
//    public IEnumerable<K> Keys => states.Keys;
//    public IEnumerable<S> Values => states.Values;
//    public int Count => states.Count;
//    #endregion

//    #region ReadonlyDictionary_Methods
//    public bool ContainsKey(K _key)
//    {
//        return states.ContainsKey(_key);
//    }
//    public IEnumerator<KeyValuePair<K, S>> GetEnumerator()
//    {
//        return states.GetEnumerator();
//    }
//    public bool TryGetValue(K _key, out S _value)
//    {
//        return states.TryGetValue(_key, out _value);
//    }
//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return states.GetEnumerator();
//    }
//    #endregion

//    #region  StateMachine_Methods
//    public void UpdateStateMachine() => TransitionToState(CurrentState == null ? default : CurrentState.UpdateState());

//    public Transition TransitionToState(K _key)
//    {
//        Transition _transition = GetTransition(_key);
//        if (_transition) FinalizeTransitionToState(_key);
//        return _transition;
//    }

//    protected virtual Transition GetTransition(K _key) => Transition.Try(this, _from: currentKey, _to: _key);


//    protected virtual void FinalizeTransitionToState(K _key)
//    {
//        CurrentState.OnStateExit();
//        currentKey = _key;
//        CurrentState.OnStateEnter();
//    }

//    #endregion

//    public class Transition
//    {
//        public Transition(K _from, K _to, bool _transition)
//        {
//            this.from = _from;
//            this.to = _to;
//            this.transition = _transition;
//        }
//        public readonly bool transition;
//        public readonly K from, to;

//        public static implicit operator bool(Transition _transition) => _transition.transition;
//        public static bool CanTransitionTo(StateMachine<K, S> _stateMachine, K _from, K _to) => !_from.Equals(_to) && _stateMachine != null && _stateMachine.ContainsKey(_from) && _stateMachine.ContainsKey(_to);
//        public static Transition Try(StateMachine<K, S> _stateMachine, K _from, K _to) => new(_from, _to, CanTransitionTo(_stateMachine, _from, _to));
//    }
//}
