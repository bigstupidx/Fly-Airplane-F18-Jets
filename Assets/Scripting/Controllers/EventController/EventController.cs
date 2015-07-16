/*
 * Event Controller (for Unity3D) v 1.2
 * 
 * author: Radomir Slaboshpitsky, VallVerk game dev. studio
 * e-mail: radiys92@gamil.com
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventController : MonoBehaviour 
{
    public delegate void Func(string EventName, GameObject Sender);
    public static EventController Instance { get; private set; }

    private struct EventPair
    {
        public string Name;
        public Func Events;
    }

    private EventPair[] _pairs;
    private Func _onAll = (a,b) => {};

    public EventController()
    {
        Instance = this;
        _pairs = new EventPair[0];
    }

    public void Subscribe(string EventName, IEventSubscriber Subscriber)
    {
        bool been = false;
        for (int i=0; i<_pairs.Length; i++)
        {
            if (_pairs[i].Name == EventName)
            {
                _pairs[i].Events += Subscriber.OnEvent;
                been = true;
                break;
            }
        }
        if (!been)
        {
            EventPair x = new EventPair();
            x.Name = EventName;
            x.Events = (a,b) => {};
            x.Events += Subscriber.OnEvent;
            Array.Resize<EventPair>(ref _pairs,_pairs.Length+1);
            _pairs[_pairs.Length-1] = x;
        }
    }

    public void SubscribeToAllEvents(IEventSubscriber Subscciber)
    {
        _onAll += Subscciber.OnEvent;
    }

    public void UnsubscribeToAllEvents(IEventSubscriber Subscciber)
    {
        _onAll -= Subscciber.OnEvent;
    }

    public void Unsubscribe(string EventName, IEventSubscriber Subscriber)
    {
        for (int i=0; i<_pairs.Length; i++)
        {
            if (_pairs[i].Name == EventName)
            {
                _pairs[i].Events -= Subscriber.OnEvent;
                break;
            }
        }
    }

    public void Unsubscribe(IEventSubscriber Subscriber)
    {
        for (int i=0; i<_pairs.Length; i++)
        {
            _pairs[i].Events -= Subscriber.OnEvent;
        }
    }

    public void PostEvent(string EventName,GameObject Sender)
    {
        for (int i=0; i<_pairs.Length; i++)
        {
            if (_pairs[i].Name == EventName)
            {
                _pairs[i].Events(EventName,Sender);
                break;
            }
        }
        _onAll(EventName, Sender);
    }
}
