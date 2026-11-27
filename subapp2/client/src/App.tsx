import { NavLink, Navigate, Route, Routes } from 'react-router-dom'

function Page({ title }: { title: string }) {
  return <h1>{title}</h1>
}

function App() {
  return (
    <>
      <nav>
        <NavLink to="/groups">Grupper</NavLink>{' | '}
        <NavLink to="/sessions">Sesjoner</NavLink>{' | '}
        <NavLink to="/students">Studenter</NavLink>
      </nav>

      <main>
        <Routes>
          <Route path="/" element={<Navigate to="/groups" replace />} />
          <Route path="/groups" element={<Page title="Grupper" />} />
          <Route path="/groups/:id" element={<Page title="Gruppedetaljer" />} />
          <Route path="/sessions" element={<Page title="Sesjoner" />} />
          <Route path="/sessions/:id" element={<Page title="Sesjonsdetaljer" />} />
          <Route path="/students" element={<Page title="Studenter" />} />
          <Route path="/students/:id" element={<Page title="Studentdetaljer" />} />
          <Route path="*" element={<Page title="Siden finnes ikke" />} />
        </Routes>
      </main>
    </>
  )
}

export default App