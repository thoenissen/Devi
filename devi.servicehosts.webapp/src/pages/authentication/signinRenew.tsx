// React
import * as React from 'react'

// Services
import { useAuth } from '../../provider/authentication/authenticationProvider'

const SigninCallback: React.FC = () => {
    const { onSilentSignin } = useAuth()

    onSilentSignin?.()

    return <h1>Signing in...</h1>
}

export default SigninCallback
