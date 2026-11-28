export interface Group {
  id: number
  name: string | null
  maxSize: number
  courseSessionId: number
  courseSessionName: string | null
  memberCount: number
}

export interface GroupMember {
  studentId: number
  name: string
  joinedAt: string
}

export interface GroupDetails extends Group {
  members: GroupMember[]
}