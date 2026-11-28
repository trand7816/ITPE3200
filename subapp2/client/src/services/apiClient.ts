const apiBaseUrl = import.meta.env.VITE_API_URL

export async function apiFetch(
  path: string,
  options: RequestInit = {},
): Promise<Response> {
  if (!apiBaseUrl) {
    throw new Error('VITE_API_URL is not configured.')
  }

  const response = await fetch(`${apiBaseUrl}${path}`, options)

  if (!response.ok) {
    const errorMessage = await response.text()
    throw new Error(
      errorMessage || `API request failed with status ${response.status}`,
    )
  }

  return response
}