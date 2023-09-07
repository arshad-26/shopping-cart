function GetUserTimeZone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone
}