using System.Collections.Generic;

public class CircularBuffer<T> where T : class
{
    private Queue<T> _queue;
    private int _size;

    public CircularBuffer(int size){
        _queue = new Queue<T>(size);
        _size = size;
    }

    public void Add(T obj){
        if(_queue.Count == _size){
            _queue.Dequeue();
            _queue.Enqueue(obj);
        }
        else
            _queue.Enqueue(obj);
    }

    public void Dequeue(){
        if (_queue.Count != 0){
            _queue.Dequeue();
        }
    }

    public T Peek(){
        if (_queue.TryPeek(out T result)){
            return result;
        }
        else
            return null;
    }

    public int Count(){
        return _queue.Count;
    }
}
