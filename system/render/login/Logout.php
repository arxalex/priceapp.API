<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class Logout extends BaseRenderBuilder
{
    public function build()
    {
        unset($_COOKIE["userid"]);
        unset($_COOKIE["token"]);
        unset($_COOKIE["token_expires"]);
        setcookie("userid", null, -1, '/');
        setcookie("token", null, -1, '/');
        setcookie("token_expires", null, -1, '/');

        header("Location: /login", true);
        die();
    }
}
