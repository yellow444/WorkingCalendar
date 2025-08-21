import react from 'react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import './App.css';
import { RadioGroup, Radio, FormControlLabel, PaletteMode, Switch } from '@mui/material';
import CssBaseline from '@mui/material/CssBaseline';
import { blue, lightBlue, grey, common } from "@mui/material/colors";
import { useTranslation } from 'react-i18next';
import './i18n';
import { GB, RU } from 'country-flag-icons/react/3x2';
import { useLocalStorage } from './useLocalStorage';
import React from 'react';
import { ym } from 'react-metrika';

function App(this: any) {

    const [text, setText] = React.useState<string>('');
    const [selectedDaysValue, setSelectedDaysValue] = useLocalStorage('selectedDaysValue', '5');
    const daysHandleRadioChange = (event: { target: { value: react.SetStateAction<string>; }; }) => {
        setSelectedDaysValue(String(event.target.value));
    };
    const [selectedDateValue, setSelectedDateValue] = useLocalStorage('selectedDateValue', 'mssql');
    const dateHandleRadioChange = (event: { target: { value: react.SetStateAction<string>; }; }) => {
        setSelectedDateValue(String(event.target.value));
    };
    const [selectedYearValue, setSelectedYearValue] = useLocalStorage('selectedYearValue', 'all');
    const yearHandleRadioChange = (event: { target: { value: react.SetStateAction<string>; }; }) => {
        setSelectedYearValue(String(event.target.value));
    };
    const [sw, setSw] = useLocalStorage('sw', false);
    const [width, setWidth] = useLocalStorage('width', Math.max(500, window.innerWidth / 2));
    const [height, setHeight] = useLocalStorage('height', Math.max(300, window.innerHeight / 3));
    const handleTextChange = (event: { target: { value: react.SetStateAction<string>; }; }) => {
        setText(String(event.target.value));
    };
    const handleMouseDownWidth = (event: { clientX: any; }) => {
        const startX = event.clientX;
        const initialWidth = width;
        const handleMouseMove = (event: { clientX: number; }) => {
            const newWidth = initialWidth + event.clientX - startX;
            setWidth(Math.max(100, newWidth));
        };
        const handleMouseUp = () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        };
        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    };
    const handleMouseDownHeight = (event: { clientY: any; }) => {
        const startY = event.clientY;
        const initialHeight = height;
        const handleMouseMove = (event: { clientY: number; }) => {
            const newHeight = initialHeight + event.clientY - startY;
            setHeight(Math.max(100, newHeight));
        };
        const handleMouseUp = () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseup', handleMouseUp);
        };
        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseup', handleMouseUp);
    };
    const [tooltiptext, setTooltiptext] = React.useState('Copy to Clipboard');
    const copyToClipboard = () => {        
        ym(97319832,'reachGoal','Copy to Clipboard');
        const textField = document.createElement('textarea');
        textField.innerText = text;
        document.body.appendChild(textField);
        textField.select();
        if (!navigator.clipboard) {
            document.execCommand('copy');
        } else {
            navigator.clipboard.writeText(text);
        }
        textField.remove();
        setTooltiptext('Copied');
    };
    const contents = text === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started.</em></p>
        :
        <div className="tooltip" style={{ width: `${width}px`, height: `${height}px`, border: '1px solid black', overflow: 'none', position: 'relative', overflowY: 'hidden', overflowX: 'hidden' }} onMouseOver={() => { setTooltiptext('Copy to Clipboard'); }} >

            <span id='tooltipCopy' className="tooltiptext"  >{tooltiptext}</span>
            <textarea value={text} onChange={handleTextChange} style={{ position: 'initial', width: '100%', height: '100%', resize: 'none', boxSizing: 'border-box', overflowX: 'hidden' }} onClick={copyToClipboard}>
            </textarea>
            <div style={{ position: 'absolute', bottom: 0, right: 0, cursor: 'ns-resize', width: '100%', height: '3px', backgroundColor: 'black' }} onMouseDown={handleMouseDownHeight}>
            </div>
            <div style={{ position: 'absolute', bottom: 0, right: 0, cursor: 'ew-resize', width: '3px', height: '100%', backgroundColor: 'black', overflowX: 'hidden' }} onMouseDown={handleMouseDownWidth}>
            </div>
        </div>
    const getDesignTokens = (mode: PaletteMode) => ({
        palette: {
            mode,
            primary: {
                main: blue[500],
                contrastText: common.white,
            },
            secondary: {
                main: lightBlue[500],
                contrastText: common.white,
            },
            ...(mode === 'light' ? {
                background: {
                    default: grey[100],
                    paper: common.white,
                },
                text: {
                    primary: grey[900],
                    secondary: grey[800],
                }
            } : {
                background: {
                    default: grey[900],
                    paper: grey[800],
                },
                text: {
                    primary: common.white,
                    secondary: grey[500],
                }
            }),
        },
        components: {
            MuiTable: {
                styleOverrides: {
                    root: {
                        ...(mode === 'dark' && {
                            backgroundColor: grey[800],
                            color: common.white,
                        }),
                    },
                },
            },
        },
    });
    const [theme, setTheme] = React.useState(createTheme(getDesignTokens('light')));
    const toggleSw = (chk: boolean) => setSw(chk);
    const [lng, setLng] = useLocalStorage<string>('lng', 'русский');
    const toggleLng = () => {
        if (lng === 'русский') {
            setLng('english');
            i18n.changeLanguage('en');
        }

        if (lng === 'english') {
            setLng('русский');
            i18n.changeLanguage('ru');
        }
    }
    const toggleLngLoad = () => {
        if (lng === 'русский') {
            setLng('русский');
            i18n.changeLanguage('ru');
        }

        if (lng === 'english') {
            setLng('english');
            i18n.changeLanguage('en');
        }
    }
    const { t, i18n } = useTranslation();
    react.useEffect(() => {
        toggleLngLoad();
        sw ? setTheme(createTheme(getDesignTokens('dark'))) : setTheme(createTheme(getDesignTokens('light')));
        async function populateWeatherData() {
            try {
                const response = await fetch(`/WorkingCalendar/GetYearWorkingCalendar?year=${selectedYearValue}&type=${selectedDateValue}&days=${selectedDaysValue}`);
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                const data = await response.text();
                setText(data);
            } catch (error) {
                console.error('Ошибка загрузки данных', error);
                setText('Ошибка загрузки данных');
            }
        };
        populateWeatherData();
        document.title = t('title'); 
    }, [selectedDaysValue, selectedDateValue, selectedYearValue, sw, lng]);
    return (

        <ThemeProvider theme={theme}>
            <CssBaseline />


            <div  >
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>


                    <span>
                        <h3>{t('Switch theme')}</h3>
                        <div className="switch">
                            <label onClick={() => {
                                setTheme(createTheme(getDesignTokens('light')));
                                toggleSw(false);
                            }}
                                title="Change theme" >{t('light')}</label>
                            <Switch
                                onChange={() => {
                                    theme.palette.mode === 'light' ?
                                        setTheme(createTheme(getDesignTokens('dark'))) :
                                        setTheme(createTheme(getDesignTokens('light')));
                                    toggleSw(!sw);
                                }}
                                checked={sw}
                                title="Change theme"
                            />
                            <label onClick={() => {
                                setTheme(createTheme(getDesignTokens('dark')));
                                toggleSw(true);
                            }}
                                title="Change theme" >{t('dark')}</label>
                        </div>
                    </span>
                    <span className='flag' onClick={() => {
                        toggleLng();
                    }} >
                        <label>{ }</label>

                        {(() => {
                            if (lng === 'english') {
                                return <RU title={lng} height="30" width="30" />
                            }
                            else {
                                return <GB title={lng} height="30" width="30" />
                            }
                        })()}
                    </span>
                </div>

                <div >
                    <h1 id="tabelLabel">{t('hello')}</h1>
                    <h2>
                        {t('welcome')}
                    </h2>
                    <table>
                        <thead>
                            <tr>
                                <th>
                                    <h4>{t('workweek')}</h4>
                                    <div>
                                        <RadioGroup aria-label="days" name="days" value={selectedDaysValue} onChange={daysHandleRadioChange} style={{ display: 'inline' }} >
                                            <FormControlLabel key={5} value="5" control={<Radio />} label="5" defaultChecked />
                                            <FormControlLabel key={6} value="6" control={<Radio />} label="6" />
                                        </RadioGroup>
                                    </div>
                                </th>
                                <th>
                                    <div>
                                        <RadioGroup aria-label="database" name="database" value={selectedDateValue} onChange={dateHandleRadioChange} style={{ display: 'inline' }} >
                                            <FormControlLabel key={'mssql'} value="mssql" control={<Radio />} label="Microsoft SQL Server" defaultChecked />
                                            <FormControlLabel key={'mysql'} value="mysql" control={<Radio />} label="MySQL" />
                                            <FormControlLabel key={'postgresql'} value="postgresql" control={<Radio />} label="PostgreSQL" />
                                        </RadioGroup>
                                    </div>
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td >
                                    <h4>{t('years')}</h4>
                                    <div style={{ overflowY: 'scroll', height: `${height}px`, }} >
                                        <RadioGroup aria-label="year" name="year" value={selectedYearValue} onChange={yearHandleRadioChange} style={{ display: 'grid' }} >
                                            <FormControlLabel key={'all'} value="all" control={<Radio />} label="All" defaultChecked />
                                            {(() => {
                                                const options = [];
                                                for (let i = 2025; i >= 2013; i--) {
                                                    options.push(<FormControlLabel key={i} value={i} control={<Radio />} label={i} />);
                                                }
                                                return options;
                                            })()}
                                        </RadioGroup>
                                    </div>
                                </td>
                                <td>
                                    <div>
                                        {contents}
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div >
            <footer style={{ marginTop: '20px', textAlign: 'center' }}>
                <a href="https://t.me/magnMaks" target="_blank" rel="noopener noreferrer" style={{ display: 'inline-flex', alignItems: 'center' }}>
                    <img 
                        src="https://www.gravatar.com/avatar/31c5543c1734d25c7206f5fd591525d0295bec6fe84ff82f946a34fe970a1e66?s=40&d=identicon" 
                        alt="Gravatar" 
                        style={{ borderRadius: '50%', marginRight: '10px' }} 
                    />
                    contact me
                </a>

                

            </footer>
        </ThemeProvider >
        

    );


}

export default App;
