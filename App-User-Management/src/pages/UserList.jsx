import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import {
  getUsers,
  deleteUser
} from "../services/userService";

function UserList() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [toast, setToast] = useState("");

  const location = useLocation();
  const navigate = useNavigate();

  const loadUsers = async () => {
    try {
      setLoading(true);
      setError("");

      const data = await getUsers();

      setUsers(Array.isArray(data) ? data : []);

    } catch (err) {
      console.error("Failed to load users:", err);

      if (err.response?.status === 401) {
        setError("You are not authorized to view users.");
      } else {
        setError("Failed to load users. Please try again.");
      }

    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();

    if (location.state?.successMessage) {

      setToast(location.state.successMessage);

      navigate(location.pathname, {
        replace: true,
        state: {}
      });

      const timer = setTimeout(() => {
        setToast("");
      }, 3000);

      return () => clearTimeout(timer);
    }
  }, []);


  const handleEdit = (id) => {
    navigate(`/edit/${id}`);
  };

  const handleDelete = async (id) => {

    const confirmed = window.confirm(
      "Are you sure you want to delete this user?"
    );

    if (!confirmed) {
      return;
    }

    try {

      await deleteUser(id);

      setUsers((currentUsers) =>
        currentUsers.filter(
          (user) => user.id !== id
        )
      );

      setToast("User deleted successfully.");

      const timer = setTimeout(() => {
        setToast("");
      }, 3000);

      return () => clearTimeout(timer);

    } catch (error) {

      console.error("Delete failed:", error);

      if (error.response?.status === 401) {

        alert(
          "You are not authorized to delete users."
        );

      } else if (error.response?.status === 404) {
        alert("User was not found.");
      } else {
        alert("Failed to delete user.");
      }
    }
  };

  if (loading) {
    return (
      <div className="container">
        <h2>Users</h2>
        <p>Loading users...</p>
      </div>
    );
  }
  
  return (
    <div className="container">
      {toast && (
        <div className="toast">
          {toast}
        </div>
      )}

      <h2>Users</h2>
      {error && (
        <div className="error-box">

          <span>{error}</span>

          <button onClick={loadUsers}>
            Retry
          </button>

        </div>
      )}

      {/* Empty */}
      {!error && users.length === 0 && (
        <div className="empty-state">
          No users found.
        </div>
      )}

      {!error && users.length > 0 && (

        <table className="user-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Age</th>
              <th>City</th>
              <th>State</th>
              <th>Pincode</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            {users.map((user) => (
              <tr key={user.id}>
                <td>{user.name}</td>
                <td>{user.age}</td>
                <td>{user.city}</td>
                <td>{user.state}</td>
                <td>{user.pincode}</td>
                <td>
                  <button type="button" onClick={() => handleEdit(user.id)}>
                    Edit
                  </button>
                  <button type="button" onClick={() => handleDelete(user.id) }>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default UserList;