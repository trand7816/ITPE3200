import type { Session, SessionDetails } from '../types/session'
import { apiFetch } from './apiClient'

export async function getSessions(): Promise<Session[]> {
  const response = await apiFetch('/api/session')
  return response.json()
}

export async function getSession(id: number): Promise<SessionDetails> {
  const response = await apiFetch(`/api/session/${id}`)
  return response.json()
}