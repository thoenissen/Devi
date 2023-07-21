// React
import React from 'react'

// Services
import { useAuth } from '../../provider/authentication/authenticationProvider'

const HomePage: React.FC = () => {
    const { isLoggedIn, onLogin, onLogout } = useAuth()

    return (
        <div>
            <h1 style={{ fontSize: '4em' }}>Welcome to Devi&apos;s garden!</h1>

            <div>
                {isLoggedIn == false ? (
                    <button type="button" onClick={() => onLogin?.()}>
                        Sign In
                    </button>
                ) : (
                    <button type="button" onClick={() => onLogout?.()}>
                        Sign Out
                    </button>
                )}
            </div>
        </div>
    )
}

export default HomePage
