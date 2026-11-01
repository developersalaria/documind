import { useState, type FormEvent } from 'react'
import { ApiError, askQuestion, uploadDocument, type AskResponse, type IngestionResult } from './api.ts'
import './App.css'

function errorMessage(error: unknown): string {
  return error instanceof ApiError ? error.message : 'Something went wrong. Please try again.'
}

function App() {
  const [file, setFile] = useState<File | null>(null)
  const [uploading, setUploading] = useState(false)
  const [uploaded, setUploaded] = useState<IngestionResult | null>(null)
  const [uploadError, setUploadError] = useState<string | null>(null)

  const [question, setQuestion] = useState('')
  const [asking, setAsking] = useState(false)
  const [answer, setAnswer] = useState<AskResponse | null>(null)
  const [askError, setAskError] = useState<string | null>(null)

  async function handleUpload(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!file) return

    setUploading(true)
    setUploadError(null)
    try {
      setUploaded(await uploadDocument(file))
    } catch (error) {
      setUploaded(null)
      setUploadError(errorMessage(error))
    } finally {
      setUploading(false)
    }
  }

  async function handleAsk(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!question.trim()) return

    setAsking(true)
    setAskError(null)
    setAnswer(null)
    try {
      setAnswer(await askQuestion(question, uploaded?.documentId))
    } catch (error) {
      setAskError(errorMessage(error))
    } finally {
      setAsking(false)
    }
  }

  return (
    <main className="app">
      <header className="app-header">
        <h1>DocuMind</h1>
        <p>Upload a document, then ask questions about it. Answers cite their sources.</p>
      </header>

      <section className="card" aria-labelledby="upload-title">
        <h2 id="upload-title">1. Upload a document</h2>
        <form className="row" onSubmit={handleUpload}>
          <input
            type="file"
            accept=".txt,.md"
            aria-label="Choose a document"
            onChange={(event) => setFile(event.target.files?.[0] ?? null)}
          />
          <button type="submit" disabled={!file || uploading}>
            {uploading ? 'Uploading…' : 'Upload'}
          </button>
        </form>
        <p className="hint">Supported today: .txt and .md. PDF support is next on the roadmap.</p>
        {uploadError && (
          <p className="message error" role="alert">
            {uploadError}
          </p>
        )}
        {uploaded && (
          <p className="message success" role="status">
            <strong>{uploaded.fileName}</strong> is ready: {uploaded.pageCount} page(s), {uploaded.chunkCount} chunk(s).
          </p>
        )}
      </section>

      <section className="card" aria-labelledby="ask-title">
        <h2 id="ask-title">2. Ask a question</h2>
        <form className="row" onSubmit={handleAsk}>
          <input
            type="text"
            value={question}
            placeholder="What is this document about?"
            aria-label="Your question"
            onChange={(event) => setQuestion(event.target.value)}
          />
          <button type="submit" disabled={!question.trim() || asking}>
            {asking ? 'Thinking…' : 'Ask'}
          </button>
        </form>
        {askError && (
          <p className="message error" role="alert">
            {askError}
          </p>
        )}
        {answer && (
          <div className="answer">
            <p>{answer.answer}</p>
            {answer.citations.length > 0 && (
              <ol className="citations">
                {answer.citations.map((citation, index) => (
                  <li key={`${citation.documentId}-${citation.pageNumber}-${index}`}>
                    <strong>{citation.fileName}</strong>, page {citation.pageNumber}: “{citation.snippet}”
                  </li>
                ))}
              </ol>
            )}
          </div>
        )}
      </section>

      <footer className="app-footer">
        Built with .NET 10, React and PostgreSQL + pgvector ·{' '}
        <a href="https://github.com/developersalaria/documind">Source on GitHub</a>
      </footer>
    </main>
  )
}

export default App
