Implementing Authentication 🔐 & Authorization 🔑 in **Blazor Server Without Identity**

When the authentication logic is implemented in the backend, Identity should not directly access the database

🧠 We know that Identity internally uses:

```csharp
HttpContext.SignInAsync();
```

to sign in the user.

🚀 Therefore, we can call this method directly ourselves — just like Identity does.

👇 Here’s what happens:
🌈 It creates an **authentication cookie** that stores user **claims**.
🌈 The **authentication middleware** takes care of validating the cookie, keeping our code clean and simple.
🌈 We only need to add this line at the top of any protected page:

```razor
@attribute [Authorize]
```

This attribute automatically checks the user’s permissions based on the authentication cookie.
If the user doesn’t have access, they’ll be redirected to the **AccessDenied** page.

🌈 You can also use the built-in **`<AuthorizeView>`** component in the UI to conditionally show content based on user roles or permissions.

---

### 🛡 Security Advantages

* Using `@attribute [Authorize]` prevents unauthorized users from even **rendering** the page, improving security.
* Since the cookie is created on the **server side** and has `HttpOnly = true`:
  💎 JavaScript or any malicious script **cannot read it**, making it resistant to **XSS attacks**.
  💎 ASP.NET Core also includes built-in **Anti-Forgery protection**, keeping the cookie safe from **CSRF attacks**.

---

### 🛸 Important Note

Because `HttpContext` can only be modified in **server-side rendered (SSR)** pages — not in **interactive (Blazor Server)** ones —
you should **not specify a `rendermode`** for the login page, so it defaults to SSR.

> ⚠️ Note: Since SSR pages are not interactive and events don’t work there, you must use an **`EditForm`** component for handling the login form submission.
