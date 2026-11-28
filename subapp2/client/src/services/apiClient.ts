const apiBaseUrl = import.meta.env.VITE_API_URL

export type ValidationErrors = Record<string, string[]>

export class ApiError extends Error {
  readonly status: number
  readonly validationErrors: ValidationErrors

  constructor(
    message: string,
    status: number,
    validationErrors: ValidationErrors = {},
  ) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.validationErrors = validationErrors
  }
}

export function getAuthHeaders(): Record<string, string> {
  const token = localStorage.getItem('token')

  return token ? { Authorization: `Bearer ${token}` } : {}
}

export async function apiFetch(
  path: string,
  options: RequestInit = {},
): Promise<Response> {
  if (!apiBaseUrl) {
    throw new Error('VITE_API_URL is not configured.')
  }

  const headers = new Headers(options.headers)

  for (const [name, value] of Object.entries(getAuthHeaders())) {
    headers.set(name, value)
  }

  return fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers,
  })
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null
}

async function readErrorBody(response: Response): Promise<unknown> {
  const text = await response.text()

  if (!text) {
    return undefined
  }

  try {
    return JSON.parse(text) as unknown
  } catch {
    return text
  }
}

function getValidationErrors(body: unknown): ValidationErrors {
  if (!isRecord(body) || !isRecord(body.errors)) {
    return {}
  }

  const errors: ValidationErrors = {}

  for (const [field, value] of Object.entries(body.errors)) {
    const messages = Array.isArray(value)
      ? value.filter((message): message is string => typeof message === 'string')
      : typeof value === 'string'
        ? [value]
        : []

    if (messages.length > 0) {
      errors[field] = messages
    }
  }

  return errors
}

function getErrorMessage(body: unknown): string | undefined {
  if (typeof body === 'string' && body.trim()) {
    return body
  }

  if (isRecord(body)) {
    for (const key of ['detail', 'title', 'message']) {
      const value = body[key]

      if (typeof value === 'string' && value.trim()) {
        return value
      }
    }
  }

  return undefined
}

function getStatusMessage(status: number): string {
  switch (status) {
    case 400:
      return 'The request was invalid.'
    case 401:
      return 'You must log in to continue.'
    case 403:
      return 'You do not have permission to do that.'
    case 404:
      return 'The requested resource was not found.'
    case 500:
      return 'The server encountered an error.'
    default:
      return `API request failed with status ${status}.`
  }
}

export function handleResponse<T>(response: Response): Promise<T>
export function handleResponse(
  response: Response,
  hasBody: false,
): Promise<void>
export async function handleResponse<T>(
  response: Response,
  hasBody = true,
): Promise<T | void> {
  if (!response.ok) {
    const body = await readErrorBody(response)
    const validationErrors =
      response.status === 400 ? getValidationErrors(body) : {}
    const message = getErrorMessage(body) ?? getStatusMessage(response.status)

    throw new ApiError(message, response.status, validationErrors)
  }

  if (!hasBody) {
    return
  }

  if (response.status === 204) {
    throw new ApiError(
      'Expected a response body but received 204 No Content.',
      response.status,
    )
  }

  return response.json()
}