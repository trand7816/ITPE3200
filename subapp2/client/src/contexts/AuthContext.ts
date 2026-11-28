import { createContext, useContext } from 'react'

type AuthContextValue = {
  token: string | null
  setToken: (token: string | null) => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
)

export function useAuth() {
  const context = useContext(AuthContext)

  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider')
  }

  return context
}