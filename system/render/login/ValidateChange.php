<?php

namespace render\login;

use render\defaultBuild\BaseRenderBuilder;

class ValidateChange extends BaseRenderBuilder
{
    public function defaultParams()
    {
        return [
            'cookie' => []
        ];
    }
    public function build()
    {
        $this->_usersService->unavaliableRedirect($this->getParam('cookie'), 1);

        if($_GET['old_password'] == "" || $_GET['new_password'] == ""){
            http_response_code(403);
            die();
        }

        $oldPassword = $_GET['old_password'];
        $newPassword = $_GET['new_password'];

        $changed = $this->_usersService->changePassword($this->getParam('cookie'), $oldPassword, $newPassword);

        if($changed){
            header("Location: /", true);
            die();
        } else {
            header("Location: /account_change", true);
            die();
        }
    }
}
