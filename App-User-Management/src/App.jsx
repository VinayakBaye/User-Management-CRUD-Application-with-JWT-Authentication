import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { useEffect, useState } from "react";

import Navbar from "./components/Navbar";
import AddUser from "./pages/AddUser";
import UserList from "./pages/UserList";
import { login } from "./services/authService";

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [authenticating, setAuthenticating] = useState(true);

    useEffect(() => {
        const authenticate = async () => {
            try {
                await login();

                console.log("Authentication successful");

                setIsAuthenticated(true);
            } catch (error) {
                console.error("Authentication failed", error);

                setIsAuthenticated(false);
            } finally {
                setAuthenticating(false);
            }
        };

        authenticate();
    }, []);

    if (authenticating) {
        return <div>Authenticating...</div>;
    }

    if (!isAuthenticated) {
        return <div>Authentication failed. Please try again.</div>;
    }

    return (
        <BrowserRouter>
            <Navbar />

           <Routes>

  <Route
    path="/list"
    element={<UserList />}
  />

  <Route
    path="/add"
    element={<AddUser />}
  />

  <Route
    path="/edit/:id"
    element={<AddUser />}
  />

  <Route
    path="/"
    element={<Navigate to="/list" replace />}
  />

  <Route
    path="*"
    element={<Navigate to="/list" replace />}
  />

</Routes>
        </BrowserRouter>
    );
}

export default App;