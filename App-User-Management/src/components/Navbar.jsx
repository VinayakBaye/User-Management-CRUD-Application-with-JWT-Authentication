import { NavLink } from "react-router-dom";

function Navbar() {
  return (
    <nav className="navbar">
      <div className="navbar-brand">
        User Management
      </div>

      <div className="navbar-links">
        <NavLink
          to="/list"
          className={({ isActive }) =>
            isActive ? "nav-link active" : "nav-link"
          }
        >
          List
        </NavLink>

        <NavLink
          to="/add"
          className={({ isActive }) =>
            isActive ? "nav-link active" : "nav-link"
          }
        >
          Add
        </NavLink>
      </div>
    </nav>
  );
}

export default Navbar;