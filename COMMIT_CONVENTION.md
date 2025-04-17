# Quy Tắc Viết Commit Message

Tài liệu này mô tả quy tắc viết commit message để đảm bảo tính nhất quán, dễ hiểu và hỗ trợ theo dõi lịch sử thay đổi trong dự án. Một commit message tốt giúp nhóm dễ dàng hiểu được mục đích của thay đổi và hỗ trợ quản lý mã nguồn hiệu quả.

## 1. Cấu Trúc Commit Message
Commit message bao gồm 3 phần chính:
1. **Type**: Loại thay đổi (feat, fix, docs, style, refactor, test, chore).
2. **Scope**: Phạm vi thay đổi (tên module, file, hoặc chức năng).
3. **Description**: Mô tả ngắn gọn về thay đổi.

Cú pháp:
```
type(scope): description
```

- **Type**:
  - `feat`: Thêm tính năng mới.
  - `fix`: Sửa lỗi.
  - `docs`: Cập nhật tài liệu (ví dụ: README, hướng dẫn).
  - `style`: Thay đổi định dạng code (không ảnh hưởng logic).
  - `refactor`: Tái cấu trúc code (không thêm tính năng, không sửa lỗi).
  - `test`: Thêm hoặc sửa test case.
  - `chore`: Các thay đổi nhỏ (cập nhật dependency, cấu hình).

- **Scope**: Tên module, file, hoặc chức năng bị ảnh hưởng (ví dụ: `auth`, `ui`, `api`).
- **Description**: Mô tả ngắn gọn (dưới 50 ký tự nếu có thể), bắt đầu bằng động từ ở thì hiện tại.

## 2. Ví Dụ Commit Message
- Thêm tính năng đăng nhập:
  ```
  feat(auth): add user login functionality
  ```
- Sửa lỗi hiển thị giao diện:
  ```
  fix(ui): correct button alignment on mobile
  ```
- Cập nhật tài liệu:
  ```
  docs(readme): update installation instructions
  ```
- Tái cấu trúc code:
  ```
  refactor(api): simplify user validation logic
  ```
- Thêm test case:
  ```
  test(auth): add unit tests for login endpoint
  ```
- Cập nhật dependency:
  ```
  chore(deps): update express to version 4.18.2
  ```

## 3. Quy Tắc Bổ Sung
- Viết commit message bằng **tiếng Anh** để đảm bảo tính chuyên nghiệp và dễ chia sẻ.
- Mô tả rõ ràng, tránh viết chung chung như "fix bug" hoặc "update code".
- Nếu commit liên quan đến issue, thêm mã issue vào cuối:
  ```
  fix(ui): resolve button overlap issue #123
  ```
- Sử dụng dấu hai chấm `:` sau `type(scope)` và để trống một khoảng trắng trước mô tả.
- Nếu cần giải thích chi tiết, thêm nội dung vào phần thân (body) của commit:
  ```
  git commit -m "feat(auth): add user login functionality
  
  - Implement JWT-based authentication
  - Add login form validation
  - Update user model with new fields"
  ```

## 4. Tại Sao Phải Tuân Theo Quy Tắc Này?
- **Tính nhất quán**: Giúp nhóm dễ dàng đọc và hiểu lịch sử commit.
- **Tự động hóa**: Hỗ trợ các công cụ như changelog generator hoặc semantic release.
- **Dễ theo dõi**: Dễ dàng tìm kiếm và phân loại các thay đổi.

## 5. Công Cụ Hỗ Trợ
- Sử dụng [Commitizen](https://commitizen-tools.github.io/commitizen/) để tạo commit message theo chuẩn:
  ```bash
  npm install -g commitizen
  cz
  ```
- Cấu hình [Conventional Commits](https://conventionalcommits.org/) trong dự án để kiểm tra commit message.

Hãy đảm bảo mỗi commit message đều rõ ràng và tuân theo quy tắc này để tối ưu hóa quy trình làm việc nhóm!