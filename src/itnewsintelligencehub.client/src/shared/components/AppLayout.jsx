import { NavLink, Outlet } from 'react-router-dom';

export function AppLayout() {
    return (
        <>
            <header className="app-navigation">
                <div className="app-navigation-content">
                    <NavLink className="app-brand" to="/sources">
                        IT News Intelligence Hub
                    </NavLink>

                    <nav className="app-navigation-links" aria-label="Main navigation">
                        <NavLink
                            className={({ isActive }) =>
                                isActive
                                    ? 'navigation-link navigation-link-active'
                                    : 'navigation-link'
                            }
                            to="/sources"
                        >
                            News sources
                        </NavLink>

                        <NavLink
                            className={({ isActive }) =>
                                isActive
                                    ? 'navigation-link navigation-link-active'
                                    : 'navigation-link'
                            }
                            to="/news"
                        >
                            News items
                        </NavLink>
                    </nav>
                </div>
            </header>

            <Outlet />
        </>
    );
}