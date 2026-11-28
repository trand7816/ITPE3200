import type { GroupDetails } from '../types/group'
import type { JoinCodeRequest } from '../types/join'
import type { SessionDetails } from '../types/session'
import { apiFetch, handleResponse } from './apiClient'

export async function joinSession(
  request: JoinCodeRequest,
): Promise<SessionDetails | GroupDetails> {
  const response = await apiFetch('/api/join', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })

  return handleResponse<SessionDetails | GroupDetails>(response)
}

export async function joinGroup(
  groupId: number,
  request: JoinCodeRequest,
): Promise<void> {
  const response = await apiFetch(`/api/join/group/${groupId}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })

  return handleResponse(response, false)
}

export async function leaveGroup(groupId: number): Promise<void> {
  const response = await apiFetch(`/api/join/leave/${groupId}`, {
    method: 'POST',
  })

  return handleResponse(response, false)
}