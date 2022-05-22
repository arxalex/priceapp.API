<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class ValidateRegister extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        if ($_GET['token'] == "" || $_GET['userid'] == "") {
            if ($_GET['confirmed'] == "") {
                return "
                <p>Якщо ви бычите цю сторынку - вам необхідно пыдтвердити вашу адресу електронної пошти</p>
                <p>На вашу пошту ми відправили лист з посиланням для підтвердження реєстрації</p>
            ";
            } else {
                return "
                <p>Адресу вашої електронної пошти підтверджено</p>";
            }
            die();
        }

        if ($this->_usersService->isLoggedInUser($this->getParam('cookie'))) {
            header("Location: /", true);
            die();
        }

        $token = $_GET['token'];
        $userid = $_GET['userid'];

        $confirmed = $this->_usersService->confirmEmail($token, $userid);

        if ($confirmed) {
            header("Location: /register/confirm_email?confirmed=1", true);
            die();
        } else {
            header("Location: /register/confirm_email", true);
            die();
        }
    }
}
