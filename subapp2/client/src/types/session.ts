import type { GroupDetails } from './group'

export interface Session {
  id: number
  name: string
  joinCode: string
  maxGroups: number
  groupSize: number
  randomAssignment: boolean
}

export interface SessionDetails extends Session {
  groups: GroupDetails[]
}