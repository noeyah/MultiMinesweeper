﻿using System.Collections.Concurrent;

namespace ServerCore;

class SessionManager
{
	private static int _sessionID = 0;

	private readonly ConcurrentDictionary<int, Session> _dicSession = new ConcurrentDictionary<int, Session>();

	private readonly int _bufferSize;

	public SessionManager(int bufferSize)
	{
		_bufferSize = bufferSize;
	}

	public Session GetSession(int sessionID)
	{
		if (_dicSession.TryGetValue(sessionID, out Session session))
		{
			return session;
		}
		return null;
	}

	public Session Create()
	{
		var session = new Session(_bufferSize);
		var id = Interlocked.Increment(ref _sessionID);
		session.SetID(id);

		_dicSession.TryAdd(id, session);
		return session;
	}

	public void Close(int sessionID)
	{
		_dicSession.TryRemove(sessionID, out var session);
	}

	public void CloseAll()
	{
		foreach (var session in _dicSession.Values)
		{
			// 서버 종료로 인한 강제 끊기!
			session.Close();
		}
	}
}
