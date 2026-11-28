import type { Group } from './group'

export interface Student {
  id: number
  name: string
}

export interface StudentDetails extends Student {
  groups: Group[]
}