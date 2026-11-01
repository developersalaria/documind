export type IngestionResult = {
  documentId: string
  fileName: string
  pageCount: number
  chunkCount: number
}

export type Citation = {
  documentId: string
  fileName: string
  pageNumber: number
  snippet: string
}

export type AskResponse = {
  answer: string
  citations: Citation[]
}

type ProblemDetails = {
  title?: string
  detail?: string
}

export class ApiError extends Error {}

async function toApiError(response: Response): Promise<ApiError> {
  try {
    const problem = (await response.json()) as ProblemDetails
    return new ApiError(problem.detail ?? problem.title ?? `Request failed (${response.status})`)
  } catch {
    return new ApiError(`Request failed (${response.status})`)
  }
}

async function send<T>(url: string, init: RequestInit): Promise<T> {
  let response: Response
  try {
    response = await fetch(url, init)
  } catch {
    throw new ApiError('Cannot reach the API. Is it running on http://localhost:5080?')
  }

  if (!response.ok) {
    throw await toApiError(response)
  }

  return (await response.json()) as T
}

export function uploadDocument(file: File): Promise<IngestionResult> {
  const body = new FormData()
  body.append('file', file)
  return send<IngestionResult>('/api/documents', { method: 'POST', body })
}

export function askQuestion(question: string, documentId?: string): Promise<AskResponse> {
  return send<AskResponse>('/api/chat/ask', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ question, documentId }),
  })
}
