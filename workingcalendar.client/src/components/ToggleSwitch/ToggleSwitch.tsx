import React from "react";
import styles from "./ToggleSwitch.module.css";

interface ToggleSwitchProps {
  label: string;
}

const ToggleSwitch: React.FC<ToggleSwitchProps> = ({ label }) => {
  return (
    <div className={styles.container}>
      {label}{" "}
      <div className={styles["toggle-switch"]}>
        <input type="checkbox" className={styles.checkbox} name={label} id={label} />
        <label className={styles.label} htmlFor={label}>
          <span className={styles.inner} />
          <span className={styles.switch} />
        </label>
      </div>
    </div>
  );
};

export default ToggleSwitch;
