import type { Group, GroupDetails } from '../types/group'
import { apiFetch, handleResponse } from './apiClient'

export interface GroupFilters {
  search?: string
  sessionId?: number
}

type GroupInput = Pick<Group, 'name' | 'maxSize' | 'courseSessionId'>

export async function getGroups(
  filters: GroupFilters = {},
): Promise<Group[]> {
  const params = new URLSearchParams()

  if (filters.search !== undefined) {
    params.set('search', filters.search)
  }

  if (filters.sessionId !== undefined) {
    params.set('sessionId', String(filters.sessionId))
  }

  const query = params.toString()
  const path = query ? `/api/group?${query}` : '/api/group'
  const response = await apiFetch(path)

  return handleResponse<Group[]>(response)
}

export async function getGroup(id: number): Promise<GroupDetails> {
  const response = await apiFetch(`/api/group/${id}`)
  return handleResponse<GroupDetails>(response)
}

export async function createGroup(group: GroupInput): Promise<Group> {
  const response = await apiFetch('/api/group', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(group),
  })

  return handleResponse<Group>(response)
}

export async function updateGroup(
  id: number,
  group: GroupInput,
): Promise<Group> {
  const response = await apiFetch(`/api/group/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(group),
  })

  return handleResponse<Group>(response)
}

export async function deleteGroup(id: number): Promise<void> {
  const response = await apiFetch(`/api/group/${id}`, {
    method: 'DELETE',
  })

  return handleResponse(response, false)
}

export async function addGroupMember(
  groupId: number,
  studentId: number,
): Promise<void> {
  const response = await apiFetch(
    `/api/group/${groupId}/members/${studentId}`,
    { method: 'POST' },
  )

  return handleResponse(response, false)
}

export async function removeGroupMember(
  groupId: number,
  studentId: number,
): Promise<void> {
  const response = await apiFetch(
    `/api/group/${groupId}/members/${studentId}`,
    { method: 'DELETE' },
  )

  return handleResponse(response, false)
}