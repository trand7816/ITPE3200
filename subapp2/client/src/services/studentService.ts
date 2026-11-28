import type { Student, StudentDetails } from '../types/student'
import { apiFetch } from './apiClient'

export async function getStudents(): Promise<Student[]> {
  const response = await apiFetch('/api/student')
  return response.json()
}

export async function getStudent(id: number): Promise<StudentDetails> {
  const response = await apiFetch(`/api/student/${id}`)
  return response.json()
}