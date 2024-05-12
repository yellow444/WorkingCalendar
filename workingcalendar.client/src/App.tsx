import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [forecasts, setForecasts] = useState<string>();

    useEffect(() => {
        populateWeatherData();
    }, []);

    const contents = forecasts === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <div className='bio' ><p>{forecasts}</p></div>

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
        setForecasts(data);
    }
}

export default App;