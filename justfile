bash := if os_family() == 'windows' {
    'bash'
} else {
    '/usr/bin/env bash'
}

