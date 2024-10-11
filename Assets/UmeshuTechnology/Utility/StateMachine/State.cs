//using System;

//public abstract class State<K> where K : Enum
//{
//    public State(StateMachine<K, State<K>> _stateMachine)
//    {
//        this.stateMachine = _stateMachine;
//        Create(_stateMachine);
//        _stateMachine.onDestroy += Destroy;
//    }

//    public readonly StateMachine<K, State<K>> stateMachine;
//    public abstract K Self { get; }

//    protected abstract void Create<S>(StateMachine<K, S> _stateMachine) where S : State<K>;
//    protected abstract void Destroy<S>(StateMachine<K, S> _stateMachine) where S : State<K>;

//    public abstract void OnStateEnter();
//    public abstract void OnStateExit();
//    public abstract K UpdateState();
//}
