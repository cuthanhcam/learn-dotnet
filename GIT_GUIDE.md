# Hướng Dẫn Sử Dụng Git

Tài liệu này cung cấp hướng dẫn chi tiết về cách sử dụng Git để quản lý mã nguồn trong dự án. Mục tiêu là đảm bảo tất cả thành viên trong nhóm tuân theo quy trình làm việc thống nhất, giảm thiểu lỗi, và tối ưu hóa cộng tác. Tài liệu bao gồm giải thích các lệnh Git, các tham số (flag/option) phổ biến, và quy trình làm việc với branch.

## 1. Cài Đặt Git
- Tải và cài đặt Git từ [trang chính thức](https://git-scm.com/).
- Cấu hình thông tin cá nhân:
  ```bash
  git config --global user.name "Tên của bạn"
  git config --global user.email "email@example.com"
  ```
- **Tham số**:
  - `--global`: Áp dụng cấu hình cho toàn bộ người dùng trên máy.
  - `--local`: Áp dụng cấu hình chỉ cho repository hiện tại (mặc định nếu không chỉ định `--global`).
  - Ví dụ: `git config --local user.name "Tên khác"` để đặt tên khác chỉ cho một repository.

## 2. Khởi Tạo Repository
- **Khởi tạo repository mới**:
  ```bash
  git init
  ```
  - **Tham số**:
    - `--bare`: Tạo repository không chứa working directory, thường dùng cho server.
    - Ví dụ: `git init --bare` để tạo một repository chỉ lưu trữ lịch sử Git.
- **Sao chép repository từ remote**:
  ```bash
  git clone https://github.com/username/repository.git
  ```
  - **Tham số**:
    - `--branch <tên-branch>`: Sao chép một branch cụ thể.
    - `--depth <số>`: Chỉ sao chép số commit gần nhất (shallow clone).
    - Ví dụ: `git clone --branch develop --depth 1 https://github.com/username/repository.git` để sao chép branch `develop` với lịch sử 1 commit.

## 3. Quy Trình Làm Việc Với Branch
Sử dụng mô hình **Git Flow** đơn giản:
- `main`: Branch chính, chứa mã ổn định, sẵn sàng triển khai.
- `develop`: Branch tích hợp các tính năng mới, dùng để kiểm thử trước khi hợp nhất vào `main`.
- `feature/*`: Branch để phát triển tính năng (ví dụ: `feature/login-page`).
- `bugfix/*`: Branch để sửa lỗi (ví dụ: `bugfix/fix-login-error`).

### Tạo và làm việc với branch
1. Tạo branch mới:
   ```bash
   git checkout -b feature/tên-tính-năng
   ```
   - **Tham số**:
     - `-b`: Tạo và chuyển sang branch mới ngay lập tức.
     - Ví dụ: `git checkout -b feature/add-user-auth` tạo và chuyển sang branch `feature/add-user-auth`.
2. Đẩy branch lên remote:
   ```bash
   git push origin feature/add-user-auth
   ```
   - **Tham số**:
     - `--set-upstream` (hoặc `-u`): Liên kết branch cục bộ với branch trên remote.
     - Ví dụ: `git push --set-upstream origin feature/add-user-auth` để thiết lập theo dõi.

### Hợp nhất branch
1. Chuyển về branch `develop`:
   ```bash
   git checkout develop
   ```
2. Kéo mã mới nhất:
   ```bash
   git pull origin develop
   ```
   - **Tham số**:
     - `--rebase`: Kéo mã và áp dụng các commit cục bộ lên trên lịch sử remote.
     - Ví dụ: `git pull --rebase origin develop` để tránh merge commit.
3. Hợp nhất branch tính năng:
   ```bash
   git merge feature/add-user-auth
   ```
   - **Tham số**:
     - `--no-ff`: Buộc tạo merge commit, giữ lịch sử branch rõ ràng.
     - `--squash`: Gộp tất cả commit của branch thành một commit duy nhất.
     - Ví dụ: `git merge --no-ff feature/add-user-auth` để giữ lịch sử branch.

## 4. Các Lệnh Git Cơ Bản và Tham Số
### Kiểm tra trạng thái
```bash
git status
```
- **Tham số**:
  - `-s` (hoặc `--short`): Hiển thị trạng thái ngắn gọn.
  - `-b` (hoặc `--branch`): Hiển thị thông tin branch hiện tại.
  - Ví dụ: `git status -s` để xem danh sách file thay đổi ngắn gọn.

### Thêm file vào staging
```bash
git add tên_file
```
- **Tham số**:
  - `.`: Thêm tất cả file đã thay đổi.
  - `-u`: Chỉ thêm các file đã được theo dõi (tracked).
  - `-A` (hoặc `--all`): Thêm tất cả file, kể cả file mới và đã xóa.
  - Ví dụ: `git add -A` để thêm mọi thay đổi.

### Tạo commit
```bash
git commit -m "Mô tả commit theo quy tắc COMMIT_CONVENTION.md"
```
- **Tham số**:
  - `-m`: Chỉ định nội dung commit message.
  - `-a`: Tự động thêm các file đã thay đổi (tracked) vào staging trước khi commit.
  - `--amend`: Sửa đổi commit trước đó (thay thế message hoặc thêm file).
  - Ví dụ: `git commit -a -m "fix: update login validation"` để commit tất cả file đã thay đổi.

### Đẩy mã lên remote
```bash
git push origin branch-name
```
- **Tham số**:
  - `-f` (hoặc `--force`): Ép đẩy, ghi đè lịch sử remote (dùng cẩn thận).
  - `--force-with-lease`: Ép đẩy nhưng kiểm tra xung đột trước.
  - Ví dụ: `git push --force-with-lease origin feature/add-user-auth` để đẩy an toàn hơn.

### Kéo mã từ remote
```bash
git pull origin branch-name
```
- **Tham số**:
  - `--rebase`: Áp dụng commit cục bộ lên lịch sử remote.
  - `--no-rebase`: Sử dụng merge thay vì rebase.
  - Ví dụ: `git pull --rebase origin develop` để đồng bộ mà không tạo merge commit.

## 5. Xử Lý Xung Đột (Merge Conflict)
Khi hợp nhất branch mà có xung đột, Git sẽ tạm dừng và yêu cầu giải quyết:
1. Mở file bị xung đột (Git đánh dấu các phần xung đột):
   ```plaintext
   <<<<<<< HEAD
   Code của bạn
   =======
   Code từ branch khác
   >>>>>>> branch-name
   ```
2. Chỉnh sửa file, giữ lại code mong muốn, xóa các ký hiệu `<<<<<<<`, `=======`, `>>>>>>>`.
3. Đánh dấu file đã giải quyết:
   ```bash
   git add tên_file
   ```
4. Hoàn tất merge:
   ```bash
   git commit
   ```
   - **Tham số**:
     - `--no-edit`: Sử dụng message mặc định cho merge commit.
     - Ví dụ: `git commit --no-edit` để hoàn tất merge mà không chỉnh sửa message.

## 6. Các Lệnh Hữu Ích Khác
### Xem lịch sử commit
```bash
git log --oneline --graph
```
- **Tham số**:
  - `--oneline`: Hiển thị mỗi commit trên một dòng.
  - `--graph`: Hiển thị lịch sử dưới dạng đồ thị.
  - `--author=<tên>`: Lọc commit theo tác giả.
  - `--since=<ngày>`: Lọc commit từ ngày cụ thể.
  - Ví dụ: `git log --oneline --author="Tên của bạn" --since="2025-04-01"` để xem commit của bạn từ 1/4/2025.

### Hủy thay đổi chưa commit
```bash
git restore tên_file
```
- **Tham số**:
  - `--staged`: Xóa file khỏi staging nhưng giữ thay đổi trong working directory.
  - Ví dụ: `git restore --staged tên_file` để bỏ staging.

### Quay về commit trước
```bash
git revert mã_commit
```
- **Tham số**:
  - `-n` (hoặc `--no-commit`): Tạo thay đổi nhưng không commit ngay.
  - Ví dụ: `git revert -n abc123` để kiểm tra trước khi commit.

### Xóa branch
```bash
git branch -d tên_branch
git push origin --delete tên_branch
```
- **Tham số**:
  - `-d` (hoặc `--delete`): Xóa branch nếu đã được merge.
  - `-D`: Xóa branch ngay cả khi chưa merge (dùng cẩn thận).
  - `--delete`: Xóa branch trên remote.
  - Ví dụ: `git branch -D feature/old-branch` để xóa branch chưa merge.

## 7. Lưu Ý
- Luôn kéo mã mới nhất trước khi làm việc: `git pull`.
- Tạo branch riêng cho từng tính năng hoặc lỗi.
- Viết commit message rõ ràng, tuân theo `COMMIT_CONVENTION.md`.
- Đẩy mã thường xuyên để tránh mất dữ liệu.
- Kiểm tra kỹ trước khi merge vào `develop` hoặc `main`.
- Sử dụng các tham số phù hợp để tối ưu hóa lệnh Git và tránh lỗi.

## 8. Tài Nguyên Tham Khảo
- [Pro Git Book](https://git-scm.com/book/en/v2): Sách miễn phí về Git.
- [Git Documentation](https://git-scm.com/docs): Tài liệu chính thức.
- [Atlassian Git Tutorials](https://www.atlassian.com/git): Hướng dẫn thực hành.

Nếu bạn cần thêm hướng dẫn hoặc giải thích về bất kỳ lệnh Git nào, hãy liên hệ với quản trị viên repository!