import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import {
  createUser,
  getUser,
  updateUser
} from "../services/userService";

function AddUser() {
  const navigate = useNavigate();
  const { id } = useParams();
  const isEditMode = Boolean(id);

  const [formData, setFormData] = useState({
    name: "",
    age: "",
    city: "",
    state: "",
    pincode: ""
  });

  const [errors, setErrors] = useState({});
  const [submitting, setSubmitting] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!isEditMode) {
      return;
    }

    const loadUser = async () => {
      try {
        setLoading(true);
        setErrors({});

        const user = await getUser(id);

        setFormData({
          name: user.name ?? "",
          age: user.age ?? "",
          city: user.city ?? "",
          state: user.state ?? "",
          pincode: user.pincode ?? ""
        });

      } catch (error) {
        console.error("Failed to load user:", error);

        if (error.response?.status === 404) {
          setErrors({
            submit: "User not found."
          });
        } else if (error.response?.status === 401) {
          setErrors({
            submit: "You are not authorized."
          });
        } else {
          setErrors({
            submit: "Failed to load user. Please try again."
          });
        }
      } finally {
        setLoading(false);
      }
    };

    loadUser();
  }, [id, isEditMode]);

  const validateField = (name, value) => {
  const trimmedValue = String(value).trim();
  let error = "";

  switch (name) {

    case "name":
      if (!trimmedValue) {
        error = "Name is required.";
      } else if (trimmedValue.length < 2 || trimmedValue.length > 100) {
        error = "Name must be between 2 and 100 characters.";
      }
      break;

    case "age":
      if (value === "" || value === null || value === undefined) {
        error = "Age is required.";
      } else if (!Number.isInteger(Number(value))) {
        error = "Age must be an integer.";
      } else if (Number(value) < 0 || Number(value) > 120) {
        error = "Age must be between 0 and 120.";
      }
      break;

    case "city":
      if (!trimmedValue) {
        error = "City is required.";
      }
      break;

    case "state":
      if (!trimmedValue) {
        error = "State is required.";
      }
      break;

    case "pincode":
      if (!trimmedValue) {
        error = "Pincode is required.";
      } else if (
        trimmedValue.length < 4 ||
        trimmedValue.length > 10
      ) {
        error = "Pincode must be between 4 and 10 characters.";
      }
      break;

    default:
      break;
  }

  return error;
};

  const validateForm = () => {
    const newErrors = {};

    Object.keys(formData).forEach((field) => {
      const error = validateField(
        field,
        formData[field]
      );

      if (error) {
        newErrors[field] = error;
      }
    });

    setErrors(newErrors);

    return Object.keys(newErrors).length === 0;
  };

  const handleBlur = (event) => {
    const { name, value } = event.target;

    const error = validateField(name, value);

    setErrors((previous) => ({
      ...previous,
      [name]: error
    }));
  };

  const handleChange = (event) => {
    const { name, value } = event.target;

    setFormData((previous) => ({
      ...previous,
      [name]: value
    }));

    setErrors((previous) => ({
      ...previous,
      [name]: ""
    }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const valid = validateForm();

    if (!valid) {
      return;
    }

    try {
      setSubmitting(true);

      const user = {
        name: formData.name.trim(),
        age: Number(formData.age),
        city: formData.city.trim(),
        state: formData.state.trim(),
        pincode: formData.pincode.trim()
      };

      if (isEditMode) {
        await updateUser(id, user);

        navigate("/list", {
          state: {
            successMessage: "User updated successfully."
          }
        });

      } else {

        await createUser(user);

        navigate("/list", {
          state: {
            successMessage: "User created successfully."
          }
        });
      }

    } catch (error) {

      console.error("Save failed:", error);

      if (error.response?.status === 401) {

        setErrors({
          submit: "You are not authorized."
        });

      } else if (error.response?.status === 404) {

        setErrors({
          submit: "User not found."
        });

      } else if (error.response?.status === 409) {

        setErrors({
          submit: "A user with these details already exists."
        });

      } else {

        setErrors({
          submit: isEditMode
            ? "Failed to update user. Please try again."
            : "Failed to create user. Please try again."
        });
      }

    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="container">
        <h2>Update User</h2>
        <p>Loading user...</p>
      </div>
    );
  }

  return (
    <div className="container">

      <h2>
        {isEditMode ? "Update User" : "Add User"}
      </h2>

      <form
        onSubmit={handleSubmit}
        className="user-form"
      >

        <div className="form-group">

          <label>Name</label>

          <input
            type="text"
            name="name"
            value={formData.name}
            onChange={handleChange}
            onBlur={handleBlur}
            maxLength={100}
          />

          {errors.name && (
            <span className="field-error">
              {errors.name}
            </span>
          )}

        </div>

        <div className="form-group">

          <label>Age</label>

          <input
            type="number"
            name="age"
            value={formData.age}
            onChange={handleChange}
            onBlur={handleBlur}
            min="0"
            max="120"
          />

          {errors.age && (
            <span className="field-error">
              {errors.age}
            </span>
          )}

        </div>

        <div className="form-group">

          <label>City</label>

          <input
            type="text"
            name="city"
            value={formData.city}
            onChange={handleChange}
            onBlur={handleBlur}
          />

          {errors.city && (
            <span className="field-error">
              {errors.city}
            </span>
          )}

        </div>

        <div className="form-group">

          <label>State</label>

          <input
            type="text"
            name="state"
            value={formData.state}
            onChange={handleChange}
            onBlur={handleBlur}
          />

          {errors.state && (
            <span className="field-error">
              {errors.state}
            </span>
          )}

        </div>

        <div className="form-group">

          <label>Pincode</label>

          <input
            type="text"
            name="pincode"
            value={formData.pincode}
            onChange={handleChange}
            onBlur={handleBlur}
            maxLength={10}
          />

          {errors.pincode && (
            <span className="field-error">
              {errors.pincode}
            </span>
          )}

        </div>

        {errors.submit && (
          <div className="error-box">
            {errors.submit}
          </div>
        )}
  
        <button
          type="submit"
          disabled={submitting}
        >
          {submitting
            ? "Saving..."
            : isEditMode
              ? "Update User"
              : "Save User"}
        </button>

        <button
          type="button"
          onClick={() => navigate("/list")}
          disabled={submitting}
        >
          Cancel
        </button>

      </form>

    </div>
  );
}

export default AddUser;