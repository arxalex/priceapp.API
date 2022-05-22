<?php

namespace framework\entities\users;

use framework\entities\confirm_email\ConfirmEmailService;
use framework\entities\default_entities\DefaultEntitiesService;
use framework\entities\tokens\TokensService;

class UsersService extends DefaultEntitiesService
{
    private TokensService $_tokensService;
    private ConfirmEmailService $_confirmEmailService;

    public function __construct()
    {
        $this->className = self::ENTITIES_NAMESPACE . "users\\User";
        $this->tableName = "pa_users";
        $this->_tokensService = new TokensService();
        $this->_confirmEmailService = new ConfirmEmailService();
    }
    public function hasPermission(array $cookies, int $role = 9): bool
    {
        if (!empty($cookies)) {
            if (isset($cookies['userid'])) {
                $user = $this->getItemFromDB($cookies['userid']);
                if ($user->role >= $role) {
                    return true;
                }
            }
        }
        return false;
    }
    public function isLoggedInUser(array $cookies): bool
    {
        if (!empty($cookies)) {
            if (isset($cookies['userid'])) {
                $user = $this->getItemFromDB($cookies['userid']);
                if (in_array($user->role, [1, 2, 3, 4, 5, 6, 7, 8, 9])) {
                    return true;
                }
            }
        }
        return false;
    }
    public function unavaliableRedirect(array $cookies, int $role = 9): void
    {
        if (!$this->hasPermission($cookies, $role)) {
            http_response_code(403);
            die();
        }
    }

    public function unavaliableRequest(array $cookies, int $role = 9): void
    {
        if (!$this->hasPermission($cookies, $role)) {
            http_response_code(403);
            die();
        }
    }

    public function isAbleToRegister(string $username, string $email): bool
    {
        $usersFromDB = $this->getItemsFromDB([
            'username' => [$username]
        ]);
        if (!empty($usersFromDB)) {
            return false;
        }
        $usersFromDB = $this->getItemsFromDB([
            'username' => [$username]
        ]);
        if (!empty($usersFromDB)) {
            return false;
        }
        return true;
    }

    public function registerUser(string $username, string $email, string $password): bool
    {
        if (!$this->isAbleToRegister($username, $email)) {
            return false;
        }

        $user = new User(null, $username, $email, password_hash($password, PASSWORD_DEFAULT), null);

        $this->insertItemToDB($user);
        $user = $this->getItemsFromDB(["username" => $username])[0];
        $this->_confirmEmailService->createToken($user->id, $email);

        return true;
    }

    public function validateUser(string $username, string $password): int
    {
        $usersFromDB = $this->getItemsFromDB([
            'username' => [$username],
        ]);
        if (count($usersFromDB) != 1) {
            http_response_code(403);
            die();
        }

        $user = $usersFromDB[0];

        if (!password_verify($password, $user->password)) {
            http_response_code(403);
            die();
        }

        if ($user->role < 1) {
            header("Location: /register/confirm_email", true);
            die();
        }

        $token = $this->_tokensService->createToken($user->id);

        unset($_COOKIE["userid"]);
        unset($_COOKIE["token"]);
        unset($_COOKIE["token_expires"]);
        setcookie("userid", $token->userid, time() + 2592000, '/');
        setcookie("token", $token->token, $token->expires, '/');
        setcookie("token_expires", $token->expires, $token->expires, '/');

        return $user->role;
    }
    public function validateUserByEmail(string $email, string $password): int
    {
        $usersFromDB = $this->getItemsFromDB([
            'email' => [$email],
        ]);

        if (count($usersFromDB) != 1) {
            http_response_code(403);
            die();
        }

        $user = $usersFromDB[0];

        return $this->validateUser($user->username, $password);
    }

    public function changePassword(array $cookies, string $oldPassword, string $newPassword): bool
    {
        if (!$this->isLoggedInUser($cookies)) {
            return false;
        }

        if (!$this->_tokensService->isTokenValid($cookies['userid'], $cookies['token'], $cookies['token_expires'])) {
            return false;
        }

        $user = $this->getItemFromDB($cookies['userid']);

        if (!password_verify($oldPassword, $user->password)) {
            return false;
        }

        $user->password = password_hash($newPassword, PASSWORD_DEFAULT);

        $this->updateItemInDB($user);

        $token = $this->_tokensService->reValidToken($cookies['userid'], $cookies['token'], $cookies['token_expires']);

        if ($token == null) {
            return false;
        }

        unset($_COOKIE["userid"]);
        unset($_COOKIE["token"]);
        unset($_COOKIE["token_expires"]);
        setcookie("userid", $token->userid, time() + 2592000, '/');
        setcookie("token", $token->token, $token->expires, '/');
        setcookie("token_expires", $token->expires, $token->expires, '/');

        return true;
    }

    public function logoutUser(array $cookies): bool
    {
        if (!$this->_tokensService->isTokenValid($cookies['userid'], $cookies['token'], $cookies['token_expires'])) {
            return false;
        }

        $status = $this->_tokensService->deleteToken($cookies['userid'], $cookies['token'], $cookies['token_expires']);

        if (!$status) {
            return false;
        }

        unset($_COOKIE["userid"]);
        unset($_COOKIE["token"]);
        unset($_COOKIE["token_expires"]);
        setcookie("userid", null, -1, '/');
        setcookie("token", null, -1, '/');
        setcookie("token_expires", null, -1, '/');

        return true;
    }

    public function getUserInfoByCookie(array $cookies): User
    {
        $user = $this->getItemFromDB($cookies['userid']);
        $user->password = "";

        return $user;
    }

    public function confirmEmail(string $token, int $userId): bool
    {
        $result = $this->_confirmEmailService->deleteToken($userId, $token);
        $user = $this->getItemFromDB($userId);
        $user->role = 1;
        $this->updateItemInDB($user);
        return $result;
    }
}
