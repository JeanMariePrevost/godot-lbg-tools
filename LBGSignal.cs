using System;
using System.Collections.Generic;
using System.Linq;

namespace LBGTools {
	/// <summary>
	/// A "Signal/Event" object that provides a type-safe event system with various QoL features.
	/// </summary>
	//
	// TODO - Make thread safe? (e.g. thread-safe collections or locks)
	public class LBGSignal<T> {
		private readonly List<CallbackEntry<T>> _listeners = []; // The list of callbacks to be called when the event is triggered.

		/// <summary>
		/// Adds a callback to the signal.
		/// </summary>
		/// <param name="callback">The callback action to be invoked when the signal is triggered.</param>
		/// <param name="timesToTrigger">The number of times the callback should be triggered before being removed.</param>
		/// <param name="priority">
		/// The priority of the callback. Higher priority callbacks are executed first.
		/// Defaults to 0 if not specified.
		/// </param>
		public void Add(Action<T> callback, int timesToTrigger, int priority = 0) {
			var entry = new CallbackEntry<T>(callback) {
				TimesToTriggerRemaining = timesToTrigger,
				Priority = priority
			};
			_listeners.Add(entry);
		}

		/// <summary> Adds a callback for when the Trigger is fired. </summary>
		public void Add(Action<T> callback) => _listeners.Add(new CallbackEntry<T>(callback));

		/// <summary> Adds a callback to be fired exactly once and then removed. </summary>
		public void AddOnce(Action<T> callback) => Add(callback: callback, timesToTrigger: 1);

		/// <summary> Adds a callback to be fired a limited number of times before being removed. </summary>
		public void AddLimited(Action<T> callback, int timesToTrigger) => Add(callback: callback, timesToTrigger: timesToTrigger);

		/// <summary> Removes a callback for when the Trigger is fired. </summary>
		public void Remove(Action<T> callback) => _listeners.RemoveAll(cb => cb.Callback == callback);

		/// <summary> Removes all registered callbacks. </summary>
		public void Clear() => _listeners.Clear();

		/// <summary> Triggers the signal, calling all registered callbacks. </summary>
		public void Emit(T arg) {
			var toRun = _listeners
				.OrderByDescending(e => e.Priority) // Sort by priority, highest first.
				.ToList(); // Create a copy of the list to avoid modifying it while iterating.

			foreach (var entry in toRun) {
				entry.Call(arg);
			}

			// Remove any callbacks that have been marked for removal.
			_listeners.RemoveAll(cb => cb.Callback == null);
		}
	}

	/// <summary>
	/// Wrapper class for LbgSignal<T> that allows for "void" callbacks.
	/// </summary>
	/// <remarks> This has to be manually maintained to mirror the LbgSignal class. </remarks>
	public class LBGSignalVoid {
		private readonly LBGSignal<object> _signal = new();

		/// <summary> Adds a callback for when the Trigger is fired. </summary>
		public void Add(Action callback) => _signal.Add(_ => callback());

		/// <summary> Adds a callback to be fired exactly once and then removed. </summary>
		public void AddOnce(Action callback) => _signal.AddOnce(_ => callback());

		/// <summary> Removes a callback for when the Trigger is fired. </summary>
		public void Remove(Action callback) => _signal.Remove(_ => callback());

		/// <summary> Removes all registered callbacks. </summary>
		public void Clear() => _signal.Clear();

		/// <summary> Triggers the signal, calling all registered callbacks. </summary>
		public void Emit() => _signal.Emit(null);
	}

	/// <summary>
	/// A class that holds a callback and its associated properties, allowing for the storing of state related to the callback, like priority and how many times it should be called.
	/// </summary>
	/// <typeparam name="T">The type of the argument passed to the callback.</typeparam>
	/// <remarks> This class is used internally by the LbgSignal class to manage the callbacks. </remarks>
	public class CallbackEntry<T> {
		public Action<T> Callback { get; set; }
		public int Priority = 0;
		public int? TimesToTriggerRemaining = null; // If not null, the callback will be removed when this number reaches 0.

		public CallbackEntry(Action<T> callback) {
			Callback = callback;
		}

		public void Call(T arg) {
			if (Callback == null) {
				return;
			}

			Callback(arg);

			if (TimesToTriggerRemaining != null) {
				TimesToTriggerRemaining--;
				if (TimesToTriggerRemaining == 0) {
					// Mark self for removal.
					Callback = null;
				}
			}
		}
	}
}