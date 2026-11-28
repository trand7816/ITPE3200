import { Nav } from 'react-bootstrap'
import { Link } from 'react-router-dom'

export function NavMenu() {
  return (
    <Nav>
      <Nav.Link as={Link} to="/groups">Grupper</Nav.Link>
      <Nav.Link as={Link} to="/sessions">Sesjoner</Nav.Link>
      <Nav.Link as={Link} to="/students">Studenter</Nav.Link>
    </Nav>
  )
}