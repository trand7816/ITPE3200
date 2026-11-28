import type { Session, SessionDetails } from '../types/session'
import { apiFetch, handleResponse } from './apiClient'

type SessionCreateInput = Pick<
  Session,
  'name' | 'maxGroups' | 'groupSize' | 'randomAssignment'
>

type SessionUpdateInput = Pick<
  Session,
  'name' | 'maxGroups' | 'randomAssignment'
>

export async function getSessions(): Promise<Session[]> {
  const response = await apiFetch('/api/session')
  return handleResponse<Session[]>(response)
}

export async function getSession(id: number): Promise<SessionDetails> {
  const response = await apiFetch(`/api/session/${id}`)
  return handleResponse<SessionDetails>(response)
}

export async function createSession(
  session: SessionCreateInput,
): Promise<Session> {
  const response = await apiFetch('/api/session', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(session),
  })

  return handleResponse<Session>(response)
}

export async function updateSession(
  id: number,
  session: SessionUpdateInput,
): Promise<Session> {
  const response = await apiFetch(`/api/session/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(session),
  })

  return handleResponse<Session>(response)
}

export async function deleteSession(id: number): Promise<void> {
  const response = await apiFetch(`/api/session/${id}`, {
    method: 'DELETE',
  })

  return handleResponse(response, false)
}