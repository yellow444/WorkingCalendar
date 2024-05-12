import React, { Component, useRef, useEffect, useState } from 'react';
import './App.css';
import ToggleSwitch from "./components/ToggleSwitch/ToggleSwitch";
function App() {

    const [text, setText] = useState<string>();
    useEffect(() => {
        populateWeatherData();
    }, []);

    const [width, setWidth] = useState(500);
    const [height, setHeight] = useState(300);

    // Обработчики изменения текста
    const handleTextChange = (event) => {
        setText(event.target.value);
    };

    // Обработчики изменения размеров блока
    const handleMouseDownWidth = (event) => {
        const startX = event.clientX;
        const initialWidth = width;

        const handleMouseMove = (event) => {
            const newWidth = initialWidth + event.clientX - startX;
            setWidth(Math.max(100, newWidth)); // минимальная ширина блока
        };

        const handleMouseUp = () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        };

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    };

    const handleMouseDownHeight = (event) => {
        const startY = event.clientY;
        const initialHeight = height;

        const handleMouseMove = (event) => {
            const newHeight = initialHeight + event.clientY - startY;
            setHeight(Math.max(100, newHeight)); // минимальная высота блока
        };

        const handleMouseUp = () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        };

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    };
    // Состояние для хранения значения переключателя
    const [isChecked, setIsChecked] = useState(false);

    // Обработчик изменения состояния переключателя
    const handleChange = () => {
        setIsChecked(!isChecked); // Инвертируем текущее значение isChecked
    };
    const contents = text === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        :


        <div style={{ width: `${width}px`, height: `${height}px`, border: '1px solid black', overflow: 'none', position: 'relative' }}>
            <div className="left">
                <label className="container">
                    <input type="checkbox" checked={isChecked} onChange={handleChange} />
                    <span className="checkmark" style={{ marginLeft: '5px' }}></span>
                </label><a className='todoName'> {isChecked ? 'Включено' : 'Выключено'}</a>
            </div>
            <textarea value={text} onChange={handleTextChange} style={{ width: '100%', height: '100%', resize: 'none', boxSizing: 'border-box', overflow: 'auto' }} />
            <div style={{ position: 'absolute', bottom: 0, right: 0, cursor: 'ns-resize', width: '100%', height: '3px', backgroundColor: 'black' }} onMouseDown={handleMouseDownHeight}></div>
            <div style={{ position: 'absolute', bottom: 0, right: 0, cursor: 'ew-resize', width: '3px', height: '100%', backgroundColor: 'black' }} onMouseDown={handleMouseDownWidth}>

            </div>
            <div className="left">
                <label class="container">
                    <input
                        value={
                            123
                        }
                        type="checkbox"
                        name=""
                        id="" />
                    <span class="checkmark"></span>
                </label>
                <p className='todoName'> {
                    123
                } </p> </div>

        </div>

    return (
        <div>
            <h1 id="tabelLabel">Weather forecast</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );

    async function populateWeatherData() {
        const response = await fetch('WorkingCalendar/GetYearWorkingCalendar?year=2024');
        const data = await response.text();
        setText(data);
    }
}

export default App;