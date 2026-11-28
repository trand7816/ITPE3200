import type { Group } from '../types/group'
import type { Student, StudentDetails } from '../types/student'
import { apiFetch, handleResponse } from './apiClient'

type StudentInput = Pick<Student, 'name'>

export async function getStudents(): Promise<Student[]> {
  const response = await apiFetch('/api/student')
  return handleResponse<Student[]>(response)
}

export async function getStudent(id: number): Promise<StudentDetails> {
  const response = await apiFetch(`/api/student/${id}`)
  return handleResponse<StudentDetails>(response)
}

export async function getMyGroups(): Promise<Group[]> {
  const response = await apiFetch('/api/student/me')
  return handleResponse<Group[]>(response)
}

export async function createStudent(student: StudentInput): Promise<Student> {
  const response = await apiFetch('/api/student', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(student),
  })

  return handleResponse<Student>(response)
}

export async function updateStudent(
  id: number,
  student: StudentInput,
): Promise<Student> {
  const response = await apiFetch(`/api/student/${id}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(student),
  })

  return handleResponse<Student>(response)
}

export async function deleteStudent(id: number): Promise<void> {
  const response = await apiFetch(`/api/student/${id}`, {
    method: 'DELETE',
  })

  return handleResponse(response, false)
}